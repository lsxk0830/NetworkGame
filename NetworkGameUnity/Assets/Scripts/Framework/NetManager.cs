using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public static class NetManager
{
    private static Socket socket; // 定义套接字
    private static ByteArray readBuff;// 接收缓冲区
    private static Queue<ByteArray> writeQueue; // 写入队列
    public delegate void EventListener(string err); // 事件委托类型
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>(); // 事件监听列表

    private static bool isConnecting = false; // 是否正在连接

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
    #endregion Socket回调
}
