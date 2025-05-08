using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class NetManager
{
    private static Socket socket; // 定义套接字
    private static ByteArray readBuff;// 接收缓冲区
    private static Queue<ByteArray> writeQueue; // 写入队列

    public delegate void MsgListener(MsgBase msgBse); // 消息委托类型
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>(); // 消息监听列表

    private static List<MsgBase> msgList = new List<MsgBase>(); // 消息列表
    private static int msgCount = 0; // 消息列表长度
    readonly static int MAX_MESSAGE_FIRE = 10; // 每一次Update处理消息量

    private static bool isConnecting = false; // 是否正在连接
    private static bool isClosing = false; // 是否正在关闭

    public static bool isUsePing = true; // 是否启用心跳
    public static int pingInterval = 30; // 心跳间隔时间
    private static float lastPingTime = 0; // 上一次发送Ping的时间
    private static float lastPongTime = 0; // 上一次收到Pong的时间

    private static readonly object _lock = new object();
    private static CancellationTokenSource cts;

    #region 连接、关闭、Send、更新

    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public static async void ConnectAsync()
    {
        lock (_lock)
        {
            if (cts != null) return;
            cts = new CancellationTokenSource();
        }

        while (true)
        {
            Debug.Log("准备连接服务器...");

            lock (_lock)
            {
                if (socket?.Connected == true) break;
                if (isConnecting) return;
                isConnecting = true;
            }
            InitState(); // 初始化成员
            socket.NoDelay = true; // 禁用Nagle算法
            try
            {
#if UNITY_EDITOR
                socket.BeginConnect("127.0.0.1", 8888, ConnectCallback, socket);
#else
            socket.BeginConnect("111.229.57.137", 8888, ConnectCallback, socket);
#endif
                // 等待连接完成或超时
                var IsCanceled = await UniTask.WaitForSeconds(5, cancellationToken: cts.Token).SuppressCancellationThrow();
                if (IsCanceled) return;
                else Debug.Log("等待下次连接尝试...");
            }
            finally
            {
                lock (_lock) isConnecting = false;
            }
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public static void Close()
    {
        if (socket == null || !socket.Connected || isConnecting) return; // 状态判断
        if (writeQueue.Count > 0) // 还有数据在发送
            isClosing = true;
        else // 没有数据在发送
        {
            socket.Close();
            EventSystem.InvokeEvent(Events.SocketOnConnectFail, "");
        }
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="msg"></param>
    public static void Send(MsgBase msg)
    {
        if (socket == null || !socket.Connected || isConnecting || isClosing) return; // 状态判断
        // 数据编码
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        // 组装长度
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length); //组装名字
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length); // 组装消息体
        // 写入队列
        ByteArray ba = new ByteArray(sendBytes);
        int count = 0;	//writeQueue的长度
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }
        // Send
        if (count == 1)
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
    }

    /// <summary>
    /// 更新
    /// </summary>
    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }

    #endregion 连接、关闭、Send、更新

    #region Socket回调

    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log($"Socket连接成功:{((IPEndPoint)socket.RemoteEndPoint).Address},{((IPEndPoint)socket.RemoteEndPoint).Port}");
            lock (_lock)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;
                isConnecting = false;
            }
            EventSystem.InvokeEvent(Events.SocketOnConnectSuccess, "");
            //开始接收
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket Connect fail {ex.ToString()}.IP:{((IPEndPoint)socket.RemoteEndPoint).Address}");
            EventSystem.InvokeEvent(Events.SocketOnConnectFail, "服务器断开连接");
            lock (_lock) isConnecting = false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"未知错误: {ex}");
            lock (_lock)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;
                isConnecting = false;
            }
        }
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar); // 获取接收数据长度
            if (count == 0)
            {
                socket.Close();
                return;
            }
            readBuff.writeIdx += count;
            // 处理二进制消息
            OnReceiveData();
            // 继续接收数据
            if (readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket接收消息错误: {ex}");
            if
            (
                ex.SocketErrorCode == SocketError.ConnectionReset || //远程强制关闭
                ex.SocketErrorCode == SocketError.Shutdown || // 连接已关闭
                ex.SocketErrorCode == SocketError.NotConnected ||// 未建立连接
                ex.SocketErrorCode == SocketError.NetworkDown // 网络不可用
            )
                EventSystem.InvokeEvent(Events.SocketOnConnectFail, "服务器断开连接");
        }
    }

    private static void SendCallback(IAsyncResult ar)
    {
        // 获取state、EndSend
        Socket socket = (Socket)ar.AsyncState;
        if (socket == null || !socket.Connected) return; // 状态判断
        int count = socket.EndSend(ar); //EndSend
        // 获取写入队列第一条数据
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        // 完整发送
        ba.readIdx += count;
        if (ba.length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                ba = writeQueue.FirstOrDefault();
            }
        }
        // 继续发送
        if (ba != null)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }
        else if (isClosing)
            socket.Close();
    }

    #endregion Socket回调

    #region 更新消息、更新PING

    /// <summary>
    /// 更新消息
    /// </summary>
    private static void MsgUpdate()
    {
        // 初步判断，提升效率
        if (msgCount == 0) return;
        // 重复处理消息
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            MsgBase msgBase = null;
            lock (msgList)
            {
                if (msgList.Count > 0)
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }
            if (msgBase != null) // 分发消息
                EventSystem.InvokeEvent(msgBase.protoName, msgBase);
            else // 没消息了
                break;
        }
    }

    /// <summary>
    /// 更新PING,发送PING协议
    /// </summary>
    public static void PingUpdate()
    {
        if (!isUsePing) return; // 是否启用
        // 发送PING
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }
        // 检测PONG时间
        if (Time.time - lastPongTime > pingInterval * 4)
            Close();
    }

    #endregion 更新消息、更新PING

    #region 事件监听【PONG协议】

    /// <summary>
    /// 监听PONG协议
    /// </summary>
    private static void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
    }

    #endregion 事件监听【PONG协议】

    #region 私有发送,内部使用

    /// <summary>
    /// 初始化成员
    /// </summary>
    private static void InitState()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // socket
        readBuff = new ByteArray(); // 接收缓冲区
        writeQueue = new Queue<ByteArray>(); // 写入队列
        isConnecting = false; // 是否正在连接
        isClosing = false; // 是否正在关闭
        msgList = new List<MsgBase>(); // 消息列表
        msgCount = 0; // 消息列表长度
        lastPingTime = Time.time; // 上一次发送PING时间
        lastPongTime = Time.time; // 上一次收到PONG时间
        if (!msgListeners.ContainsKey("MsgPong")) // 监听PONG协议
            EventSystem.RegisterEvent("MsgPong", OnMsgPong);
    }

    /// <summary>
    /// 1、根据协议的前两个字节判断是否收到一条完整的协议。如果收到完整的协议，便解析它；
    /// 如果没有收到完整的协议，则退出等待下一波消息
    /// 2、解析协议
    /// </summary>
    private static void OnReceiveData()
    {
        if (readBuff.length <= 2) return; // 消息长度
        // 获取消息体长度
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = BitConverter.ToInt16(bytes, readIdx);
        if (readBuff.length < bodyLength + 2) return;
        readBuff.readIdx += 2;
        // 解析协议名
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out int nameCount);
        if (protoName == "")
        {
            Debug.Log($"OnReceiveData MsgBase.DecodeName Fial");
            return;
        }
        readBuff.readIdx += nameCount;
        // 解析协议体
        int bodyCount = bodyLength - nameCount;

        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        // 添加到消息队列
        lock (msgList)
        {
            msgList.Add(msgBase);
            msgCount++;
        }
        // 继续读取消息
        if (readBuff.length > 2)
            OnReceiveData();
    }

    #endregion  私有发送,内部使用
}
