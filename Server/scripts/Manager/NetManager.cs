using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

public class NetManager
{
    public static Socket listenfd; // 监听Socket
    public static ConcurrentDictionary<Socket, ClientState> clients = new ConcurrentDictionary<Socket, ClientState>(); // 客户端Socket及状态信息
    private static List<Socket> checkRead = new List<Socket>(); // Select的检查列表
    public const long pingInterval = 30; // ping间隔

    /// <summary>
    /// 开启服务端监听
    /// </summary>
    /// <param name="listenPort">监听Socket的端口号</param>
    public static async Task StartLoop(int listenPort, CancellationToken token)
    {
        try
        {
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };// 禁用Nagle算法，减少延迟
            listenfd.Bind(new IPEndPoint(IPAddress.Any, listenPort));
            listenfd.Listen(0); // 最多可容纳等待接受的连接数，0表示不限制
            Console.WriteLine($"服务器已启动");

            while (!token.IsCancellationRequested)
            {
                ResetCheckRead(); // 重置checkRead
                Socket.Select(checkRead, null, null, 1000);
                for (int i = checkRead.Count - 1; i >= 0; i--) // 检查可读对象
                {
                    Socket s = checkRead[i];
                    if (s == listenfd)
                        ReadListenfd(s);
                    else
                        ReadClientfd(s);
                }
                Timer();  // 定时
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"服务器停止: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"清理资源");
            Cleanup();
        }
    }

    /// <summary>
    /// 重置checkRead列表
    /// </summary>
    public static void ResetCheckRead()
    {
        checkRead.Clear();
        checkRead.Add(listenfd);
        checkRead.AddRange(clients.Keys);
    }

    /// <summary>
    /// 读取listenfd,新建客户端信息对象State,并存入客户端信息列表clients
    /// </summary>
    private static void ReadListenfd(Socket listenfd)
    {
        try
        {
            Socket clientfd = listenfd.Accept();
            clientfd.NoDelay = true;
            ClientState state = new ClientState()
            {
                socket = clientfd,
                lastPingTime = GetTimeStamp(),
            };

            clients.TryAdd(clientfd, state);

            Console.WriteLine($"接收{clientfd.RemoteEndPoint}的远程连接");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Accept fail {ex.ToString()}");
        }
    }

    /// <summary>
    /// 读取Clientfd
    /// </summary>
    private static void ReadClientfd(Socket clientfd)
    {
        ClientState? state;
        if (!clients.TryGetValue(clientfd, out state)) return;
        int count = 0;
        try
        {
            ByteArray readBuff = state.readBuff;
            // 缓冲区不够，清除，若依旧不够，只能返回
            // 缓冲区长度只有1024，单条协议超过缓冲区长度时会发生错误，根据需要调整长度
            if (readBuff.remain <= 0)
            {
                OnReceiveData(state);
                readBuff.MoveBytes();
            }
            if (readBuff.remain <= 0)
            {
                Console.WriteLine($"接收消息失败,maybe msg leng > buff capacity");
                Close(state);
                return;
            }

            try
            {
                count = clientfd.Receive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"接收Socekt异常: {ex.ToString()}");
                Close(state);
                return;
            }
            // 客户端关闭
            if (count <= 0)
            {
                Console.WriteLine($"关闭Socket客户端 : {clientfd.RemoteEndPoint}");
                Close(state);
                return;
            }
            // 消息处理
            readBuff.writeIdx += count;
            // 处理二进制消息
            OnReceiveData(state);
            // 移动缓冲区
            readBuff.CheckAndMoveBytes();
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"处理客户端数据时发生Socket错误: {ex.SocketErrorCode}");
            Close(state);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"处理客户端数据时发生异常: {ex.Message}");
            Close(state);
        }
    }

    public static void Close(ClientState state)
    {
        if (state == null) return;

        // 事件分发
        MethodInfo? mei = typeof(EventHandler).GetMethod("OnDisconnect");
        object[] ob = { state };
        mei?.Invoke(null, ob);
        // 关闭
        state.socket.Shutdown(SocketShutdown.Both);
        state.socket.Close();

        clients[state.socket] = null; // 清理引用，帮助垃圾回收
        clients.TryRemove(state.socket, out ClientState cs);
        state.socket = null; // 清理引用，帮助垃圾回收
    }

    /// <summary>
    /// 数据处理
    /// </summary>
    public static void OnReceiveData(ClientState state)
    {
        ByteArray readBuff = state.readBuff;
        //消息长度
        if (readBuff.length <= 2) return;
        Int16 bodyLength = readBuff.ReadInt16();
        //消息体
        if (readBuff.length < bodyLength) return;
        //解析协议名
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out int nameCount);
        if (protoName == "")
        {
            Console.WriteLine("接收数据 MsgBase.DecodeName 失败");
            Close(state);
            return;
        }
        readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        //Console.WriteLine("接收:" + protoName + "协议");
        //分发消息
        MethodInfo? mi = typeof(MsgHandler).GetMethod(protoName);
        object[] o = { state, msgBase };
        if (mi != null)
            mi.Invoke(null, o);
        else
            Console.WriteLine($"接收数据失败: {protoName}");
        //继续读取消息
        if (readBuff.length > 2)
            OnReceiveData(state);
    }

    /// <summary>
    /// 发送
    /// </summary>
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

    /// <summary>
    /// 清理资源
    /// </summary>
    private static void Cleanup()
    {
        try
        {
            // 关闭所有客户端连接
            List<ClientState> clientStates;
            clientStates = new List<ClientState>(clients.Values);
            clients.Clear();

            foreach (var state in clientStates) Close(state);

            listenfd?.Close(); // 关闭监听Socket
        }
        catch (Exception ex)
        {
            Console.WriteLine($"清理资源时发生异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 定时器
    /// </summary>
    private static void Timer()
    {
        //消息分发
        MethodInfo? mei = typeof(EventHandler).GetMethod(name: "OnTimer");
        mei?.Invoke(null, null);
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}