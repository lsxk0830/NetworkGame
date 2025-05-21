/// <summary>
/// 获取房间、创建房间、删除房间、生成MsgGetRoomList协议
/// </summary>
public class RoomManager
{
    /// <summary>
    /// 房间列表
    /// </summary>
    public static Dictionary<string, Room> rooms = new Dictionary<string, Room>();
    private static readonly Random _random = new Random();

    /// <summary>
    /// 获取房间
    /// </summary>
    public static Room GetRoom(string roomID)
    {
        if (rooms.ContainsKey(roomID))
            return rooms[roomID];
        return null;
    }

    public static Room[] GetRooms()
    {
        return rooms.Values.ToArray();
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    public static Room CreateRoom()
    {
        string roomId;
        do
        {
            roomId = GetRoomID();
        } while (rooms.ContainsKey(roomId)); // 防止冲突

        Room room = new Room();
        room.RoomID = roomId;
        rooms.Add(room.RoomID, room);
        return room;
    }

    /// <summary>
    /// 删除房间
    /// </summary>
    public static void RemoveRoom(string roomID)
    {

        var emptyRooms = new List<string>();

        foreach (var pair in rooms)
        {
            if (pair.Value.PlayerCount == 0)
                emptyRooms.Add(pair.Key);
        }

        foreach (var roomId in emptyRooms)
        {
            rooms.Remove(roomId);
            Console.WriteLine($"房间已移除: {roomId}");
        }
    }

    //    /// <summary>
    //    /// 生成MsgGetRoomList协议
    //    /// </summary>
    //    public static MsgBase ToMsg()
    //    {
    //        MsgGetRoomList msg = new MsgGetRoomList();
    //        int count = rooms.Count;
    //        msg.rooms = new RoomInfo[count];
    //        //rooms
    //        int i = 0;
    //        foreach (Room room in rooms.Values)
    //        {
    //            RoomInfo roomInfo = new RoomInfo
    //            {
    //                id = room.id,
    //                count = room.playerIds.Count,
    //                status = (int)room.status
    //            };
    //            msg.rooms[i] = roomInfo;
    //            i++;
    //        }
    //        return msg;
    //    }

    //    /// <summary>
    //    /// Update
    //    /// </summary>
    //    public static void Update()
    //    {
    //        foreach (Room room in rooms.Values)
    //            room.Update();
    //    }

    #region

    /// <summary>
    /// 生成房间ID（时间戳+4位随机数）
    /// </summary>
    private static string GetRoomID()
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        int randomNum = _random.Next(1000, 9999);
        return $"{timestamp}{randomNum}";
    }
    #endregion
}