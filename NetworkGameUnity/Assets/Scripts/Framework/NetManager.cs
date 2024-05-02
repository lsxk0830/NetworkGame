using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public static class NetManager
{
    #region 事件
    /// <summary>
    /// 事件
    /// </summary>
    public enum NetEvent
    {
        /// <summary>
        /// 连接成功
        /// </summary>
        ConnectSucc = 1,

        /// <summary>
        /// 连接失败
        /// </summary>
        ConnectFail = 2,

        /// <summary>
        /// 断开连接
        /// </summary>
        Close = 3
    }

    #endregion
    private static Socket socket; // 定义套接字
    private static ByteArray readBuff;// 接收缓冲区
    private static Queue<ByteArray> writeQueue; // 写入队列

    public delegate void EventListener(string err); // 事件委托类型
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>(); // 事件监听列表
    public delegate void MsgListener(MsgBase msgBse); // 消息委托类型
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>(); // 消息监听列表

    private static List<MsgBase> msgList = new List<MsgBase>(); // 消息列表
    private static int msgCount = 0; // 消息列表长度
    readonly static int MAX_MESSAGE_FIRE = 10; // 每一次Update处理消息量

    private static bool isConnecting = false; // 是否正在连接
    private static bool isClosing = false; // 是否正在关闭

    #region 【事件】【消息】监听、移除、分发
    #region 事件
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="netEvent">事件类型</param>
    /// <param name="listener">事件的监听</param>
    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent)) // 添加事件
            eventListeners[netEvent] += listener;
        else // 新增事件
            eventListeners[netEvent] = listener;
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="netEvent">事件类型</param>
    /// <param name="listener">事件的监听</param>
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
            if (eventListeners[netEvent] == null)
                eventListeners.Remove(netEvent);
        }
    }

    /// <summary>
    /// 分发事件
    /// </summary>
    /// <param name="netEvent">事件类型</param>
    /// <param name="err">传给回调方法的字符串</param>
    private static void FireEvent(NetEvent netEvent, string err)
    {
        if (eventListeners.ContainsKey(netEvent))
            eventListeners[netEvent](err);
    }
    #endregion 事件

    #region 消息
    /// <summary>
    /// 添加消息监听
    /// </summary>
    /// <param name="netEvent">消息名</param>
    /// <param name="listener">消息的监听</param>
    public static void AddMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName)) // 添加事件
            msgListeners[msgName] += listener;
        else // 新增事件
            msgListeners[msgName] = listener;
    }

    /// <summary>
    /// 移除消息监听
    /// </summary>
    /// <param name="netEvent">消息名</param>
    /// <param name="listener">消息的监听</param>
    public static void RemoveMsgListener(string msgName, MsgListener listene)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listene;
            if (msgListeners[msgName] == null)
                msgListeners.Remove(msgName);
        }
    }
    /// <summary>
    /// 分发消息
    /// </summary>
    /// <param name="netEvent">事件类型</param>
    /// <param name="err">传给回调方法的消息</param>
    private static void FireMsg(string msgName, MsgBase msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
            msgListeners[msgName](msgBase);
    }
    #endregion 消息

    #endregion 【事件】【消息】监听、移除、分发
    #region 连接、关闭、Send
    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public static void Connect(string ip, int port)
    {
        if (socket != null && socket.Connected)
        {
            Debug.Log("Connect Fail,already connected!");
            return;
        }

        if (isConnecting)
        {
            Debug.Log("Connect Fail,isConnecting!");
            return;
        }

        // 初始化成员
        InitState();
        // 参数设置
        socket.NoDelay = true;
        // Connect
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public static void Close()
    {
        // 状态判断
        if (socket == null || !socket.Connected) return;
        if (isConnecting) return;
        // 还有数据在发送
        if (writeQueue.Count > 0)
            isClosing = true;
        // 没有数据在发送
        else
        {
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="msg"></param>
    public static void Send(MsgBase msg)
    {
        // 状态判断
        if (socket == null || !socket.Connected) return;
        if (isConnecting) return;
        if (isClosing) return;
        // 数据编码
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        // 组装长度
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        //组装名字
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        // 组装消息体
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
        // 写入队列
        ByteArray ba = new ByteArray(sendBytes);
        int count = 0;
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
    }

    /// <summary>
    /// 更新消息
    /// </summary>
    public static void MsgUpdate()
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
                FireMsg(msgBase.protoName, msgBase);
            else // 没消息了
                break;
        }
    }

    #endregion 连接、关闭、Send

    /// <summary>
    /// 初始化成员
    /// </summary>
    private static void InitState()
    {
        // socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // 接收缓冲区
        readBuff = new ByteArray();
        // 写入队列
        writeQueue = new Queue<ByteArray>();
        // 是否正在连接
        isConnecting = false;
        // 是否正在关闭
        isClosing = false;
        // 消息列表
        msgList = new List<MsgBase>();
        // 消息列表长度
        msgCount = 0;
    }

    #region Socket回调
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ");
            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;
            //开始接收
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket Connect fail {ex.ToString()}");
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
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
            OnReveiveData();
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
            Debug.LogError($"Socket Receive fail : {ex.ToString()}");
        }
    }

    private static void SendCallback(IAsyncResult ar)
    {
        // 获取state、EndSend
        Socket socket = (Socket)ar.AsyncState;
        // 状态判断
        if (socket == null || !socket.Connected) return;
        //EndSend
        int count = socket.EndSend(ar);
        // 获取写入队列第一条数据
        ByteArray ba;
        lock (writeQueue)
            ba = writeQueue.First();
        // 完整发送
        ba.readIdx += count;
        if (ba.length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                ba = writeQueue.First();
            }
        }
        // 继续发送
        if (ba != null)
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        else if (isClosing)
            socket.Close();
    }
    #endregion Socket回调

    /// <summary>
    /// 1、根据协议的前两个字节判断是否收到一条完整的协议。如果收到完整的协议，便解析它；
    /// 如果没有收到完整的协议，则退出等待下一波消息
    /// 2、解析协议
    /// </summary>
    private static void OnReveiveData()
    {
        // 消息长度
        if (readBuff.length <= 2) return;
        // 获取消息体长度
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)(bytes[readIdx] | bytes[readIdx + 1] << 8);
        if (readBuff.length < bodyLength + 2) return;
        readBuff.readIdx += 2;
        // 解析协议名
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
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
            msgList.Add(msgBase);
        msgCount++;
        // 继续读取消息
        if (readBuff.length > 2)
            OnReveiveData();
    }
}
