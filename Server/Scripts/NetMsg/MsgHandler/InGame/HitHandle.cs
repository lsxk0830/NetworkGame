public partial class MsgHandler
{
    /// <summary>
    /// 击中协议
    /// </summary>
    public static void MsgHit(ClientState c, MsgBase msgBase)
    {
        //MsgHit msg = (MsgHit)msgBase;
        //Player player = c.player;
        //if (player == null) return;
        //// targetPlayer
        //Player targetPlayer = PlayerManager.GetPlayer(msg.targetId);
        //if (targetPlayer == null) return;
        //// room
        //Room room = RoomManager.GetRoom(player.roomId);
        //if (room == null) return;
        //// status
        //if ((Room.Status)room.status != Room.Status.FIGHT) return;
        //// 发送者校验
        //if (player.ID != msg.id) return;
        //// 状态
        //int damage = 35;
        //targetPlayer.hp -= damage;
        //// 广播
        //msg.hp = targetPlayer.hp;
        //msg.damage = damage;
        //room.Broadcast(msg);
    }
}