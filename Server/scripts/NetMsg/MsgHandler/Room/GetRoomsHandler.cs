public partial class MsgHandler
{
    /// <summary>
    /// 请求房间列表协议处理
    /// </summary>
    public static void MsgGetRooms(ClientState cs, MsgBase msgBase)
    {
        Console.WriteLine($"接收:MsgGetRooms协议");

        User? user = cs.user;
        if (user == null) return;
        NetManager.Send(cs, RoomManager.GetRoomsToMsg());
    }
}