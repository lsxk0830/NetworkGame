using System.Collections.Generic;
using System.Net.Sockets;

public static class NetManager
{
    private static Socket socket; // 定义套接字
    private static ByteArray readBuff;// 接收缓冲区
    private static Queue<ByteArray> writeQueue; // 写入队列
    public delegate void EventListener(string err); // 事件委托类型
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>(); // 事件监听列表

    #region 事件监听、移除
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
    #endregion 事件监听、移除
}
