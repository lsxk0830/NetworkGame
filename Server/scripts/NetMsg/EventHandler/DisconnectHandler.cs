public partial class EventHandler
{
    /// <summary>
    /// 离线协议处理
    /// </summary>
    /// <param name="c"></param>
    public static void OnDisconnect(ClientState c)
    {
        Console.WriteLine($"关闭Socket:{c.socket.RemoteEndPoint}");
        UserManager.RemoveUser(c);
    }

    /// <summary>
    /// 定时事件
    /// </summary>
    public static void OnTimer()
    {
        CheckPing();
        //RoomManager.Update();
    }

    /// <summary>
    /// Ping检查，最多断开一个客户端连接
    /// </summary>
    private static void CheckPing()
    {
        long timeNow = NetManager.GetTimeStamp(); //现在的时间戳
        foreach (ClientState s in NetManager.clients.Values)
        {
            if (timeNow - s.lastPingTime > NetManager.pingInterval * 4)
            {
                Console.WriteLine("Ping Close " + s.socket.RemoteEndPoint.ToString());
                NetManager.Close(s);
                return; // foreach中
            }
        }
    }
}