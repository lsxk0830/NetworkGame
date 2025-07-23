public partial class MsgHandler
{
    /// <summary>
    /// 发送场景所有障碍物协议
    /// </summary>
    public static void MsgObstacleOne(ClientState cs, MsgBase msgBase)
    {
        Console.WriteLine($"MsgObstacleOne");
        MsgObstacleOne msg = (MsgObstacleOne)msgBase;

        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;

        if (room.GetAllObstacleCount < room.playerIds.Count)
        {
            Console.WriteLine($"更新位置");
            room.SetObstaclePosRotScale(msg.ObstacleID, msg.PosRotScale);
        }
        room.BroadcastExceptCS(user.ID, msg);
    }
}