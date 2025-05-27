public partial class MsgHandler
{
    /// <summary>
    /// 战斗结束
    /// </summary>
    public static void MsgEndBattle(ClientState c, MsgBase msgBase)
    {
        //Room room = RoomManager.GetRoom(c.player.roomId);
        //if (room != null) // Room是否存在
        //{
        //    MsgEndBattle msg = (MsgEndBattle)msgBase;
        //    foreach (string id in room.playerIds.Keys)  // 统计信息
        //    {
        //        Player player = PlayerManager.GetPlayer(id);
        //        if (player.camp == msg.winCamp)
        //            player.data.Win++;
        //        else
        //            player.data.Lost++;
        //    }
        //    room.Broadcast(msg);
        //}
    }
}