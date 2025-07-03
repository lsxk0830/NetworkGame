public partial class MsgHandler
{
    private const int damagePerHit = 35; // 每次击中造成的伤害

    /// <summary>
    /// 击中协议
    /// </summary>
    public static void MsgHit(ClientState cs, MsgBase msgBase)
    {
        Console.WriteLine($"击中协议");
        MsgHit msg = (MsgHit)msgBase;

        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;
        //Player? attackPlayer = room.GetPlayer(msg.id); // 攻击者
        Player? hitPlayer = room.GetPlayer(msg.targetId);// 被击中者
        //if (attackPlayer == null) return;
        if (hitPlayer == null) return;
        if ((Room.Status)room.status != Room.Status.FIGHT) return;

        // 状态
        hitPlayer.hp -= damagePerHit;
        msg.hp = hitPlayer.hp;
        msg.damage = damagePerHit;

        room.Broadcast(msg);// 广播
    }
}