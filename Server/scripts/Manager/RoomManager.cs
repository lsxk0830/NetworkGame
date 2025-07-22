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

        Room room = new Room()
        {
            RoomID = roomId,
            mapId = 1,
        };
        rooms.Add(room.RoomID, room);
        return room;
    }

    /// <summary>
    /// 删除房间.(备注：删除房间时要不要将User的RoomID清空)
    /// </summary>
    public static void RemoveRoom(string roomID)
    {
        var emptyRooms = new List<string>();

        if (rooms.TryGetValue(roomID, out Room room))
        {
            rooms.Remove(roomID);
            room.Dispose();
            room = null;
            Console.WriteLine($"移除房间: {roomID}");
        }

        foreach (var pair in rooms)
        {
            if (pair.Value.playerIds.Count == 0)
                emptyRooms.Add(pair.Key);
        }

        foreach (var roomId in emptyRooms)
        {
            rooms.Remove(roomId);
            rooms[roomId].Dispose();
            rooms[roomId] = null;
            Console.WriteLine($"移除空房间: {roomId}");
        }
    }

    //    /// <summary>
    //    /// Update
    //    /// </summary>
    //    public static void Update()
    //    {
    //        foreach (Room room in rooms.Values)
    //            room.Update();
    //    }

    #region 网络协议

    /// <summary>
    /// 生成MsgGetRooms协议
    /// </summary>
    public static MsgBase GetRoomsToMsg()
    {
        MsgGetRooms msg = new MsgGetRooms();
        int count = rooms.Count;
        msg.rooms = new Room[count];

        int i = 0;
        foreach (Room room in rooms.Values)
        {
            msg.rooms[i] = room;
            i++;
        }
        return msg;
    }

    #endregion 网络协议

    #region 私有方法

    /// <summary>
    /// 生成房间ID（时间戳+4位随机数）
    /// </summary>
    private static string GetRoomID()
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        int randomNum = _random.Next(1000, 9999);
        return $"{timestamp}{randomNum}";
    }

    #endregion 私有方法
}