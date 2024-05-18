using System;

public partial class EventHandler
{
    public static void OnDisconnect(ClientState c)
    {
        Console.WriteLine("Close");
        //Player����
        //if (c.player != null)
        //{
        //    //��������
        //    DbManager.UpdatePlayerData(c.player.id, c.player.data);
        //    //�Ƴ�
        //    PlayerManager.RemovePlayer(c.player.id);
        //}
    }

    /// <summary>
    /// ��ʱ�¼�
    /// </summary>
    public static void OnTimer()
    {
        CheckPing();
    }

    /// <summary>
    /// Ping��飬���Ͽ�һ���ͻ�������
    /// </summary>
    public static void CheckPing()
    {
        long timeNow = NetManager.GetTimeStamp(); //���ڵ�ʱ���
        foreach (ClientState s in NetManager.clients.Values)
        {
            if (timeNow - s.lastPingTime > NetManager.pingInterval * 4)
            {
                Console.WriteLine("Ping Close " + s.socket.RemoteEndPoint.ToString());
                NetManager.Close(s);
                return; // foreach��
            }
        }
    }
}