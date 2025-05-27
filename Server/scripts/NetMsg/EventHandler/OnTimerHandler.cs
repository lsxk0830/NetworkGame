public partial class EventHandler
{
    /// <summary>
    /// 定时事件
    /// </summary>
    public static void OnTimer(double PingInterval)
    {
        CheckPing(PingInterval);
    }

    /// <summary>
    /// Ping检查，最多断开一个客户端连接
    /// </summary>
    private static void CheckPing(double PingInterval)
    {
        long timeNow = NetManager.GetTimeStamp(); //现在的时间戳
        List<ClientState> toRemove = new List<ClientState>();

        lock (NetManager.clients)
        {
            foreach (ClientState client in NetManager.clients.Values)
            {
                if (timeNow - client.lastPingTime > PingInterval * 4)
                {
                    toRemove.Add(client);
                }
            }
        }

        foreach (var client in toRemove)
        {
            Console.WriteLine($"客户端 {client.socket.RemoteEndPoint} 心跳超时");
            NetManagerSin.CloseClient(client);
        }
    }
}