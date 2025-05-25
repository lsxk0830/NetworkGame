public partial class MsgHandler
{
    /// <summary>
    /// 创建房间
    /// </summary>
    public static void MsgCreateRoom(ClientState cs, MsgBase msgBase)
    {
        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        User? user = cs.user;
        if (user == null) return;

        Room room = RoomManager.CreateRoom(); // 创建房间
        msg.result = 0;
        NetManager.Send(cs, msg); //返回创建房间的结果
        UserManager.SendExcept(cs, msg); // 全员通知
    }
}