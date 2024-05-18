using System;

public partial class EventHandler
{
    public static void OnDisconnect(ClientState c)
    {
        Console.WriteLine("Close");
        //Player下线
        //if (c.player != null)
        //{
        //    //保存数据
        //    DbManager.UpdatePlayerData(c.player.id, c.player.data);
        //    //移除
        //    PlayerManager.RemovePlayer(c.player.id);
        //}
    }

    /// <summary>
    /// 定时事件
    /// </summary>
    public static void OnTimer()
    {
        CheckPing();
    }

    /// <summary>
    /// Ping检查，最多断开一个客户端连接
    /// </summary>
    public static void CheckPing()
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