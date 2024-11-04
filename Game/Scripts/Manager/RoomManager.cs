using System.Collections.Generic;

/// <summary>
/// 获取房间、创建房间、删除房间、生成MsgGetRoomList协议
/// </summary>
public class RoomManager
{
    private static int maxId = 1; // 最大Id

    /// <summary>
    /// 房间列表
    /// </summary>
    public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();

    /// <summary>
    /// 获取房间
    /// </summary>
    public static Room GetRoom(int id)
    {
        if (rooms.ContainsKey(id))
            return rooms[id];
        return null;
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    public static Room AddRoom()
    {
        maxId++;
        Room room = new Room();
        room.id = maxId;
        rooms.Add(room.id, room);
        return room;
    }

    /// <summary>
    /// 删除房间
    /// </summary>
    public static bool RemoveRoom(int id)
    {
        rooms.Remove(id);
        return true;
    }

    /// <summary>
    /// 生成MsgGetRoomList协议
    /// </summary>
    public static MsgBase ToMsg()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        int count = rooms.Count;
        msg.rooms = new RoomInfo[count];
        //rooms
        int i = 0;
        foreach (Room room in rooms.Values)
        {
            RoomInfo roomInfo = new RoomInfo
            {
                id = room.id,
                count = room.playerIds.Count,
                status = (int)room.status
            };
            msg.rooms[i] = roomInfo;
            i++;
        }
        return msg;
    }

    /// <summary>
    /// Update
    /// </summary>
    public static void Update()
    {
        foreach (Room room in rooms.Values)
            room.Update();
    }
}