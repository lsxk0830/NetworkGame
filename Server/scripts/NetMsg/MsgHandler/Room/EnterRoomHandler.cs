public partial class MsgHandler
{
    /// <summary>
    /// 进入房间
    /// </summary>
    public static void MsgEnterRoom(ClientState c, MsgBase msgBase)
    {
        Console.WriteLine($"接收:MsgEnterRoom协议");

        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        if (c.user == null)
        {
            Console.WriteLine($"进入房间异常");
            msg.result = -1;
            NetManager.Send(c, msg);
            return;
        }

        Room? room = RoomManager.GetRoom(msg.roomID);
        if (room == null)
        {
            Console.WriteLine($"用户{c.user.ID}进入房间异常");
            msg.result = -1;
            NetManager.Send(c, msg);
        }
        else
        {
            Player player = new Player(c)
            {
                ID = c.user.ID,
                Name = c.user.Name,
                AvatarPath=c.user.AvatarPath,
            };
            c.user.RoomID = room.RoomID;
            room.EnterRoomAddPlayer(player);
        }
    }
}