public partial class MsgHandler
{
    /// <summary>
    /// 战斗结束
    /// </summary>
    public static void MsgEndBattle(ClientState cs, MsgBase msgBase)
    {
        Console.WriteLine($"战斗结束协议");
        MsgEndBattle msg = (MsgEndBattle)msgBase;

        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;

        List<User> users = new List<User>(room.playerIds.Count);
        foreach (var player in room.playerIds)  // 统计信息
        {
            User? playerUser = UserManager.GetUser(player.Key);
            if( playerUser == null) continue; // 玩家不存在
            if (player.Value.camp == msg.winCamp)
                playerUser.Win++;
            else
                playerUser.Lost++;
            users.Add(playerUser); // 添加到列表中
        }
        DbManager.BatchUpdateUsers(users); // 更新数据库
        room.Broadcast(msg);
    }
}