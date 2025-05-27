public partial class MsgHandler
{
    /// <summary>
    /// 请求开始战斗
    /// </summary>
    public static void MsgStartBattle(ClientState cs, MsgBase msgBase)
    {
        //MsgStartBattle msg = (MsgStartBattle)msgBase;

        //if (cs.user == null)
        //{
        //    Console.WriteLine("用户未登录，无法开始战斗");
        //    msg.result = -1;
        //    NetManager.Send(cs, msg);
        //    return;
        //}

        //Room room = RoomManager.GetRoom(msg.roomID);

        //if (room == null) // Room是否存在
        //{
        //    msg.result = -1;
        //    NetManager.Send(cs, msg);
        //    return;
        //}

        //if (room.ownerId != cs.user.ID) // 是否是房主
        //{
        //    msg.result = -1;
        //    NetManager.Send(cs, msg);
        //    return;
        //}
        //if (!room.StartBattle()) // 开战
        //{
        //    msg.result = -1;
        //    NetManager.Send(cs, msg);
        //    return;
        //}
        //// 成功
        //msg.result = 0;
        //NetManager.Send(cs, msg);
    }
}