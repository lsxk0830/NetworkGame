public partial class EventHandler
{
    /// <summary>
    /// 定时事件,Ping检查
    /// </summary>
    public static void OnTimer()
    {
        long timeNow = NetManager.GetTimeStamp(); //现在的时间戳

        foreach (ClientState client in NetManager.clients.Values)
        {
            if (timeNow - client.lastPingTime > NetManager.pingInterval * 4)
            {
                NetManager.Close(client);
                client.Dispose(); // 释放资源
                break; // 找到第一个超时的客户端，退出循环
            }
        }
    }
}