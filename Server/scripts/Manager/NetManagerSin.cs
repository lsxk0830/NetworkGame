using MySqlX.XDevAPI;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

public class NetManagerSin
{
    public static TimeSpan PingInterval { get; set; } = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan SelectTimeout = TimeSpan.FromMilliseconds(1000);

    public static Socket listenfd { get; private set; } // 监听Socket
    private static readonly Dictionary<Socket, ClientState> Clients = new Dictionary<Socket, ClientState>(); // 客户端Socket及状态信息
    private static readonly List<Socket> checkRead = new List<Socket>(); // Select的检查列表

    /// <summary>
    /// 开启服务端监听循环
    /// </summary>
    public static async Task StartLoopAsync(int listenPort, CancellationToken cancellationToken)
    {
        try
        {
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };// 禁用Nagle算法，减少延迟
            listenfd.Bind(new IPEndPoint(IPAddress.Any, listenPort));
            listenfd.Listen(0); // 设置合理的backlog值
            Console.WriteLine($"服务器已启动");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ResetCheckRead();

                    // 使用Poll替代Select以获得更好的性能
                    Socket.Select(checkRead, null, null, (int)SelectTimeout.TotalMilliseconds);

                    for (int i = checkRead.Count - 1; i >= 0; i--) // 检查可读对象
                    {
                        Socket s = checkRead[i];

                        if (s == listenfd)
                            ReadListenfd();
                        else
                            ReadClientfd(s);
                    }

                    Timer(); // 执行定时任务
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"主循环异常: {ex.Message}");
                    await Task.Delay(1000, cancellationToken); // 发生错误时稍作延迟
                }
            }
        }
        catch (Exception ex) when (ex is SocketException || ex is OperationCanceledException)
        {
            Console.WriteLine($"服务器停止: {ex.Message}");
        }
        finally
        {
            Cleanup();
        }
    }

    /// <summary>
    /// 重置checkRead列表
    /// </summary>
    private static void ResetCheckRead()
    {
        checkRead.Clear();
        checkRead.Add(listenfd);

        lock (Clients) // 添加线程安全保护
        {
            checkRead.AddRange(Clients.Keys);
        }
    }

    /// <summary>
    /// 读取listenfd,新建客户端信息对象State,并存入客户端信息列表clients
    /// </summary>
    private static void ReadListenfd()
    {
        try
        {
            Socket clientSocket = listenfd.Accept();
            clientSocket.NoDelay = true; // 对新连接也禁用Nagle算法

            var clientState = new ClientState
            {
                socket = clientSocket,
                lastPingTime = GetTimestamp()
            };

            lock (Clients)
            {
                Clients.Add(clientSocket, clientState);
            }

            Console.WriteLine($"接受新连接: {clientSocket.RemoteEndPoint}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"接受连接失败: {ex.SocketErrorCode}");
        }
    }

    /// <summary>
    /// 读取listenfd,新建客户端信息对象State,并存入客户端信息列表clients
    /// </summary>
    /// <param name="clientSocket"></param>
    private static void ReadClientfd(Socket clientSocket)
    {
        ClientState? state;

        lock (Clients)
        {
            if (!Clients.TryGetValue(clientSocket, out state)) return;
        }
        int count = 0; // 接收数据长度
        try
        {
            ByteArray readBuffer = state.readBuff;

            if (readBuffer.remain <= 0)
            {
                OnReceiveData(state);
                readBuffer.MoveBytes();
            }
            if (readBuffer.remain <= 0)
            {
                Console.WriteLine($"接收消息失败,maybe msg leng > buff capacity");
                CloseClient(state);
                return;
            }
            try
            {
                count = clientSocket.Receive(readBuffer.bytes, readBuffer.writeIdx, readBuffer.remain, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"接收Socekt异常: {ex.ToString()}");
                CloseClient(state);
                return;
            }

            if (count == 0) // 客户端断开连接
            {
                Console.WriteLine($"客户端 {clientSocket.RemoteEndPoint} 断开连接");
                CloseClient(state);
                return;
            }
            readBuffer.writeIdx += count; // 消息处理
            OnReceiveData(state); // 处理二进制消息
            readBuffer.CheckAndMoveBytes(); // 移动缓冲区
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"处理客户端数据时发生Socket错误: {ex.SocketErrorCode}");
            CloseClient(state);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"处理客户端数据时发生异常: {ex.Message}");
            CloseClient(state);
        }
    }

    /// <summary>
    /// 数据处理
    /// </summary>
    private static void OnReceiveData(ClientState state)
    {
        ByteArray readBuff = state.readBuff;
        if (readBuff.length <= 2) return;
        Int16 bodyLength = readBuff.ReadInt16(); //消息长度
        if (readBuff.length < bodyLength) return; //消息体
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out int nameCount);//解析协议名
        if (protoName == "")
        {
            Console.WriteLine("接收数据 MsgBase.DecodeName 失败");
            CloseClient(state);
            return;
        }
        readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        //Console.WriteLine("接收:" + protoName + "协议");
        InvokeMsg(state, msgBase, protoName); //分发消息
        if (readBuff.length > 2) //继续读取消息
            OnReceiveData(state);
    }

    public static void Send(ClientState cs, MsgBase msg)
    {
        //状态判断
        if (cs == null || !cs.socket.Connected) return;

        //数据编码
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        //组装长度
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);//组装名字
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);//组装消息体
        try
        {
            //Console.WriteLine($"发送消息：{(byte)(len % 256)}{(byte)(len / 256)}{Encoding.UTF8.GetString(nameBytes)}{Encoding.UTF8.GetString(bodyBytes)}");
            //Console.WriteLine($"消息:{Encoding.UTF8.GetString(nameBytes)}");
            cs.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null); //为简化代码，不设置回调
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Socket Close on BeginSend" + ex.ToString());
        }
    }

    public static void CloseClient(ClientState state)
    {
        if (state == null) return;

        try
        {
            InvokeEvent("OnDisconnect", state);

            state.socket.Shutdown(SocketShutdown.Both);
            state.socket.Close();

            lock (Clients)
            {
                Clients.Remove(state.socket);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"关闭客户端连接时发生异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    private static void Cleanup()
    {
        try
        {
            // 关闭所有客户端连接
            List<ClientState> clientStates;

            lock (Clients)
            {
                clientStates = new List<ClientState>(Clients.Values);
                Clients.Clear();
            }

            foreach (var state in clientStates) state.socket.Close();

            listenfd?.Close(); // 关闭监听Socket
        }
        catch (Exception ex)
        {
            Console.WriteLine($"清理资源时发生异常: {ex.Message}");
        }
    }

    private static void InvokeMsg(ClientState state, MsgBase msgBase, string protoName)
    {
        MethodInfo? mi = typeof(MsgHandler).GetMethod(protoName);
        object[] o = { state, msgBase };
        if (mi != null)
            mi.Invoke(null, o);
        else
            Console.WriteLine($"接收数据失败: {protoName}");
    }

    private static void InvokeEvent(string eventName, ClientState state)
    {
        MethodInfo? mei = typeof(EventHandler).GetMethod(eventName);// 事件分发
        object[] ob = { state };
        mei?.Invoke(null, ob);
    }

    /// <summary>
    /// 定时器
    /// </summary>
    private static void Timer()
    {
        //消息分发
        MethodInfo? mei = typeof(EventHandler).GetMethod(name: "OnTimer");
        object[] ob = { PingInterval.TotalSeconds };
        mei?.Invoke(null, ob);
    }

    /// <summary>
    /// 获取当前时间戳
    /// </summary>
    public static long GetTimestamp()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}