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

    public static void OnTimer()
    {
        //CheckPing();
    }

    //Ping���
    public static void CheckPing()
    {
        //���ڵ�ʱ���
        //long timeNow = NetManager.GetTimeStamp();
        ////������ɾ��
        //foreach (ClientState s in NetManager.clients.Values)
        //{
        //    if (timeNow - s.lastPingTime > NetManager.pingInterval * 4)
        //    {
        //        Console.WriteLine("Ping Close " + s.socket.RemoteEndPoint.ToString());
        //        NetManager.Close(s);
        //        return;
        //    }
        //}
    }
}