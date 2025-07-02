public partial class EventHandler
{
    /// <summary>
    /// 离线协议处理
    /// </summary>
    public static void OnDisconnect(ClientState cs)
    {
        Console.WriteLine($"关闭Socket:{cs.socket?.RemoteEndPoint}");
        UserManager.RemoveUser(cs);
        if (cs != null && cs.user != null && cs.user.RoomID != "")
        {
            Room room = RoomManager.GetRoom(cs.user.RoomID);
            room.RemovePlayer(cs.user.ID);
        }
    }
}