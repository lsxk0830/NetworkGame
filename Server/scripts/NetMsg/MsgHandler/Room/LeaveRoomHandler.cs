public partial class MsgHandler
{
    /// <summary>
    /// 离开房间
    /// </summary>
    public static void MsgLeaveRoom(ClientState cs, MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(msg.roomID);
        if (room == null)
        {
            msg.result = -1;
            NetManager.Send(cs, msg);
        }
        else
        {
            bool result = room.RemovePlayer(user.ID);
            // 返回协议
            msg.result = result ? 0 : -1;
            msg.ID = user.ID;
            msg.OwnerID = room.ownerId;
            NetManager.Send(cs, msg);
        }
    }
}