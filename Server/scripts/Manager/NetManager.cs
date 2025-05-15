using System.Net;
using System.Net.Sockets;
using System.Reflection;

public class NetManager
{
    public static Socket listenfd; // 监听Socket
    public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>(); // 客户端Socket及状态信息
    private static List<Socket> checkRead = new List<Socket>(); // Select的检查列表
    public static long pingInterval = 30; // ping间隔

    /// <summary>
    /// 开启服务端监听
    /// </summary>
    /// <param name="listenPort">监听Socket的端口号</param>
    public static async Task StartLoop(int listenPort, CancellationToken token)
    {
        // Socket
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // Bind
        IPAddress ipAdr = IPAddress.Parse("0.0.0.0");
        IPEndPoint ipEp = new IPEndPoint(ipAdr, listenPort);
        listenfd.Bind(ipEp);
        // Listen
        listenfd.Listen(0); // 最多可容纳等待接受的连接数，0表示不限制
        Console.WriteLine("服务器启动");

        // 循环
        try
        {
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
                //Timer();  // 定时
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("网络服务正在关闭...");
        }
    }

    /// <summary>
    /// 重置checkRead列表
    /// </summary>
    public static void ResetCheckRead()
    {
        checkRead.Clear();
        checkRead.Add(listenfd);
        foreach (ClientState s in clients.Values)
        {
            checkRead.Add(s.socket);
        }
    }

    /// <summary>
    /// 读取listenfd,新建客户端信息对象State,并存入客户端信息列表clients
    /// </summary>
    private static void ReadListenfd(Socket listenfd)
    {
        try
        {
            Socket clientfd = listenfd.Accept();
            Console.WriteLine($"接收{clientfd.RemoteEndPoint.ToString()}的远程连接");
            ClientState state = new ClientState();
            state.socket = clientfd;
            state.lastPingTime = GetTimeStamp();
            clients.Add(clientfd, state);
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
        ClientState state = clients[clientfd];
        ByteArray readBuff = state.readBuff;
        // 接收
        int count = 0;
        // 缓冲区不够，清除，若依旧不够，只能返回
        // 缓冲区长度只有1024，单条协议超过缓冲区长度时会发生错误，根据需要调整长度
        if (readBuff.remain <= 0)
        {
            OnReceiveData(state);
            readBuff.MoveBytes();
        }
        if (readBuff.remain <= 0)
        {
            Console.WriteLine($"Receive fail,maybe msg leng > buff capacity");
            Close(state);
            return;
        }

        try
        {
            count = clientfd.Receive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0);
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Receive SocketException {ex.ToString()}");
            Close(state);
            return;
        }
        // 客户端关闭
        if (count <= 0)
        {
            Console.WriteLine($"Socket close : {clientfd.RemoteEndPoint.ToString()}");
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

    public static void Close(ClientState state)
    {
        // 事件分发
        MethodInfo? mei = typeof(EventHandler).GetMethod("OnDisconnect");
        object[] ob = { state };
        mei?.Invoke(null, ob);
        // 关闭
        state.socket.Close();
        clients.Remove(state.socket);
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
            Console.WriteLine("OnReceiveData MsgBase.DecodeName fail");
            Close(state);
            return;
        }
        readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        Console.WriteLine("Receive:" + protoName);
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
    /// 定时器
    /// </summary>
    private static void Timer()
    {
        //消息分发
        MethodInfo? mei = typeof(EventHandler).GetMethod(name: "OnTimer");
        object[] ob = { };
        mei?.Invoke(null, ob);
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