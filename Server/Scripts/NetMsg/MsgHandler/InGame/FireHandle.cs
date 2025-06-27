public partial class MsgHandler
{
    /// <summary>
    /// 开火协议
    /// </summary>
    public static void MsgFire(ClientState cs, MsgBase msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;

        room.BroadcastExceptCS(cs.user.ID, msg);
    }
}