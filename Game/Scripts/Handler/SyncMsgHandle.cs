using System;

/// <summary>
/// 战场同步的消息处理逻辑
/// </summary>
public partial class MsgHandler
{
    /// <summary>
    /// 同步位置协议
    /// </summary>
    public static void MsgSyncTank(ClientState c, MsgBase msgBase)
    {
        MsgSyncTank msg = (MsgSyncTank)msgBase;
        Player player = c.player;
        if (player == null) return;
        // room 
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        // status
        if (room.status != Room.Status.FIGHT) return;
        // 是否作弊
        if (Math.Abs(player.x - msg.x) > 5 || Math.Abs(player.y - msg.y) > 5 || Math.Abs(player.z - msg.z) > 5)
            Console.WriteLine($"疑似作弊；{player.id}");
        // 更新信息
        player.x = msg.x;
        player.y = msg.y;
        player.z = msg.z;
        player.ex = msg.ex;
        player.ey = msg.ey;
        player.ez = msg.ez;
        // 广播
        msg.id = player.id;
        room.Broadcast(msg);
    }

    /// <summary>
    /// 开火协议
    /// </summary>
    public static void MsgFire(ClientState c, MsgBase msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        Player player = c.player;
        if (player == null) return;
        // room 
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        // status
        if (room.status != Room.Status.FIGHT) return;
        msg.id = player.id;
        room.Broadcast(msg);
    }

    /// <summary>
    /// 击中协议
    /// </summary>
    public static void MsgHit(ClientState c, MsgBase msgBase)
    {
        MsgHit msg = (MsgHit)msgBase;
        Player player = c.player;
        if (player == null) return;
        // targetPlayer
        Player targetPlayer = PlayerManager.GetPlayer(msg.targetId);
        if (targetPlayer == null) return;
        // room 
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        // status
        if (room.status != Room.Status.FIGHT) return;
        // 发送者校验
        if (player.id != msg.id) return;
        // 状态
        int damage = 35;
        targetPlayer.hp -= damage;
        // 广播
        msg.id = player.id;
        msg.hp = player.hp;
        msg.damage = damage;
        room.Broadcast(msg);
    }
}