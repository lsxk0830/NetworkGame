public partial class MsgHandler
{
    /// <summary>
    /// 开火协议
    /// </summary>
    public static void MsgFire(ClientState cs, MsgBase msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        User? user = cs.user;
        if (user == null) return;
        Room room = RoomManager.GetRoom(user.RoomID);
        if (room == null) return;
        if(msg.IsExplosion) // 如果炮弹爆炸，则需要发送给其他玩家
        {
            room.BroadcastExceptCS(user.ID, msg);
        }
        else // 如果是普通开火，则只发送给房间内的玩家
        {
            msg.bulletID = Guid.NewGuid();
            room.Broadcast(msg);
        }
        //Console.WriteLine($"开火协议：{JsonConvert.SerializeObject(msg)}");
    }
}