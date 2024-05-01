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

    private static bool isConnecting = false; // 是否正在连接
    private static bool isClosing = false; // 是否正在关闭

    #region 事件监听、移除、分发
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="netEvent">事件类型</param>
    /// <param name="listener">监听回调</param>
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
    /// <param name="listener">监听回调</param>
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
    #endregion 事件监听、移除、分发

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
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket Connect fail {ex.ToString()}");
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
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
}
