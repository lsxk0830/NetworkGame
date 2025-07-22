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
            Room? room = RoomManager.GetRoom(cs.user.RoomID);
            if (room != null)
            {
                room.RemovePlayer(cs.user.ID);
                if (room.status == (int)Room.Status.FIGHT)
                {
                    cs.user.Lost++;
                    DbManager.UpdateUser(cs.user);
                }
                Console.WriteLine($"用户:{cs.user.Name}离开房间:{cs.user.RoomID}");
            }
        }
    }
}