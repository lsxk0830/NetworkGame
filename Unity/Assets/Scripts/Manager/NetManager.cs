using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class NetManager : Singleton<NetManager>
{
    private Socket socket; // 定义套接字
    private ByteArray readBuff;// 接收缓冲区
    private ConcurrentQueue<ByteArray> writeQueue; // 写入队列
    private ConcurrentQueue<MsgBase> msgQueue; // 消息列表
    private int MAX_MESSAGE_FIRE = 50; // 每一次Update处理消息量
    private readonly List<MsgBase> batchList = new(90); // 批量处理消息

    private bool isConnecting = false; // 是否正在连接
    private bool isClosing = false; // 是否正在关闭

    public const int PingInterval = 4; // 心跳间隔时间30秒
    private float lastPingTime = 0; // 上一次发送Ping的时间
    private float lastPongTime = 0; // 上一次收到Pong的时间

    private readonly object _lock = new object();
    private CancellationTokenSource cts;
    private MsgPing msgPing = new MsgPing();

    #region 连接、关闭、Send、更新

    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public async void ConnectAsync()
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
                socket.BeginConnect(API.IP, 8888, ConnectCallback, socket);
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
    public void Close()
    {
        if (socket == null || !socket.Connected || isConnecting) return; // 状态判断
        if (writeQueue.Count > 0) // 还有数据在发送
            isClosing = true;
        else // 没有数据在发送
        {
            socket.Close();
            EventManager.Instance.InvokeEvent(Events.SocketOnConnectFail, "服务断开");
        }
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="msg"></param>
    public void Send(MsgBase msg)
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
        ByteArray ba = this.GetObjInstance<ByteArray>();
        ba.bytes = sendBytes;
        ba.capacity = sendBytes.Length;
        ba.initSize = sendBytes.Length;
        ba.readIdx = 0;
        ba.writeIdx = sendBytes.Length;
        writeQueue.Enqueue(ba);
        // Send
        if (writeQueue.Count == 1)
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
        MsgUpdate();
        PingUpdate();
    }

    #endregion 连接、关闭、Send、更新

    #region Socket回调

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            //Debug.Log($"Socket连接成功:{((IPEndPoint)socket.RemoteEndPoint).Address},{((IPEndPoint)socket.RemoteEndPoint).Port}");
            lock (_lock)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;
                isConnecting = false;
            }
            EventManager.Instance.InvokeEvent(Events.SocketOnConnectSuccess, "服务器连接成功");
            //开始接收
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket Connect fail {ex.ToString()}.IP:{((IPEndPoint)socket.RemoteEndPoint).Address}");
            EventManager.Instance.InvokeEvent(Events.SocketOnConnectFail, "服务器断开连接");
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

    private void ReceiveCallback(IAsyncResult ar)
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
            Debug.LogError($"Socket接收消息异常: {ex}");
            if
            (
                ex.SocketErrorCode == SocketError.ConnectionReset || //远程强制关闭
                ex.SocketErrorCode == SocketError.Shutdown || // 连接已关闭
                ex.SocketErrorCode == SocketError.NotConnected ||// 未建立连接
                ex.SocketErrorCode == SocketError.NetworkDown // 网络不可用
            )
                EventManager.Instance.InvokeEvent(Events.SocketOnConnectFail, "异常服务器断开连接");
        }
    }

    private void SendCallback(IAsyncResult ar)
    {
        // 获取state、EndSend
        Socket socket = (Socket)ar.AsyncState;
        if (socket == null || !socket.Connected) return; // 状态判断
        int count = socket.EndSend(ar); //EndSend
        // 获取写入队列第一条数据
        if (writeQueue.TryDequeue(out ByteArray ba))
        {
            ba.readIdx += count;
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
    private void MsgUpdate()
    {
        if (msgQueue.IsEmpty) return;

        float startTime = Time.realtimeSinceStartup;
        int processed = 0;
        while (processed < MAX_MESSAGE_FIRE && msgQueue.TryDequeue(out MsgBase msg))
        {
            batchList.Add(msg);

            // 时间片检查
            if (Time.realtimeSinceStartup - startTime >= 0.002f)
                break;
            processed++;
        }

        // 触发批量事件
        foreach (var msg in batchList)
        {
            EventManager.Instance.InvokeEvent(msg.protoName, msg);
        }
        batchList.Clear();

        // 动态调整下帧处理量
        MAX_MESSAGE_FIRE = (1f / Time.deltaTime) > 30 ? 90 : 50;
    }

    /// <summary>
    /// 更新PING,发送PING协议
    /// </summary>
    public void PingUpdate()
    {
        // 发送PING
        if (Time.time - lastPingTime > PingInterval)
        {
            Send(msgPing);
            lastPingTime = Time.time;
            Debug.Log($"发送Ping协议");
        }
        // 检测PONG时间
        if (Time.time - lastPongTime > PingInterval * 4)
            Close();
    }

    #endregion 更新消息、更新PING

    #region 事件监听【PONG协议】

    /// <summary>
    /// 监听PONG协议
    /// </summary>
    private void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
        Debug.Log($"接收Pong协议");
    }

    #endregion 事件监听【PONG协议】

    #region 私有发送,内部使用

    /// <summary>
    /// 初始化成员
    /// </summary>
    private void InitState()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // socket
        readBuff = this.GetObjInstance<ByteArray>();
        readBuff.bytes = new byte[1024]; // 初始化接收缓冲区
        readBuff.capacity = 1024; // 接收缓冲区大小
        readBuff.initSize = 1024; // 初始化大小
        readBuff.readIdx = 0; // 读取索引
        readBuff.writeIdx = 0; // 写入索引
        writeQueue = new ConcurrentQueue<ByteArray>(); // 写入队列
        isConnecting = false; // 是否正在连接
        isClosing = false; // 是否正在关闭
        msgQueue = new ConcurrentQueue<MsgBase>(); // 消息列表
        lastPingTime = Time.time; // 上一次发送PING时间
        lastPongTime = Time.time; // 上一次收到PONG时间
        if (!EventManager.Instance.ContainerMsgBase.ContainsKey("MsgPong")) // 监听PONG协议
            EventManager.Instance.RegisterEvent("MsgPong", OnMsgPong);
    }

    /// <summary>
    /// 1、根据协议的前两个字节判断是否收到一条完整的协议。如果收到完整的协议，便解析它；
    /// 如果没有收到完整的协议，则退出等待下一波消息
    /// 2、解析协议
    /// </summary>
    private void OnReceiveData()
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
        msgQueue.Enqueue(msgBase);
        // 继续读取消息
        if (readBuff.length > 2)
            OnReceiveData();
    }

    #endregion  私有发送,内部使用
}
