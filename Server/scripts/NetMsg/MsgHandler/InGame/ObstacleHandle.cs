public partial class MsgHandler
{
    /// <summary>
    /// 同步位置协议
    /// </summary>
    public static void MsgObstacle(ClientState cs, MsgBase msgBase)
    {
        MsgObstacle msg = (MsgObstacle)msgBase;

        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;

        room.BroadcastExceptCS(user.ID, msg);
    }
}