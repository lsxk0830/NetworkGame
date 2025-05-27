public partial class MsgHandler
{
    /// <summary>
    /// 同步位置协议
    /// </summary>
    public static void MsgSyncTank(ClientState c, MsgBase msgBase)
    {
        //MsgSyncTank msg = (MsgSyncTank)msgBase;
        //Player player = c.player;
        //if (player == null) return;
        //// room
        //Room room = RoomManager.GetRoom(player.roomId);
        //if (room == null) return;
        //// status
        //if ((Room.Status)room.status != Room.Status.FIGHT) return;
        //// 是否作弊
        //if (Math.Abs(player.x - msg.x) > 5 || Math.Abs(player.y - msg.y) > 5 || Math.Abs(player.z - msg.z) > 5)
        //    Console.WriteLine($"疑似作弊；{player.ID}");
        //// 更新信息
        //player.x = msg.x;
        //player.y = msg.y;
        //player.z = msg.z;
        //player.ex = msg.ex;
        //player.ey = msg.ey;
        //player.ez = msg.ez;
        //// 广播
        //msg.id = player.ID;
        //room.Broadcast(msg);
    }
}