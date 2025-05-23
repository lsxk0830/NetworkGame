public partial class MsgHandler
{
    /// <summary>
    /// 请求房间列表协议处理
    /// </summary>
    public static void MsgGetRooms(ClientState c, MsgBase msgBase)
    {
        Player? player = c.player;
        if (player == null)
            return;
        player.Send(RoomManager.SendRoomsToMsg());
    }
}