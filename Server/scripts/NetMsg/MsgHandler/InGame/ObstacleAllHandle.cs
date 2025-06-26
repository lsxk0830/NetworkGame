public partial class MsgHandler
{
    /// <summary>
    /// 发送场景所有障碍物协议
    /// </summary>
    public static void MsgObstacleAll(ClientState cs, MsgBase msgBase)
    {
        MsgObstacleAll msg = (MsgObstacleAll)msgBase;

        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;

        NetManager.Send(cs, room.GetAllObstacle(msg));
    }
}