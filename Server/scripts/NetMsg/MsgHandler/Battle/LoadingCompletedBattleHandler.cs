public partial class MsgHandler
{
    /// <summary>
    /// 战斗准备加载完成（客户端推送）
    /// </summary>
    public static void MsgLoadingCompletedBattle(ClientState cs, MsgBase msgBase)
    {
        MsgLoadingCompletedBattle msg = (MsgLoadingCompletedBattle)msgBase;

        if (cs.user == null)
        {
            Console.WriteLine("用户加载完成错误，无法开始战斗");
            msg.result = -1;
            NetManager.Send(cs, msg);
            return;
        }

        Room room = RoomManager.GetRoom(msg.roomID);

        if (room == null) // Room是否存在
        {
            msg.result = -1;
            NetManager.Send(cs, msg);
            return;
        }

        room.LoadSuccess();
    }
}