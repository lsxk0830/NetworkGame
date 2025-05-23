/// <summary>
/// 获取房间列表
/// </summary>
public class MsgGetRooms : MsgBase
{
    public MsgGetRooms() { protoName = "MsgGetRooms"; }

    /// <summary>
    /// 服务器返回的所有房间信息
    /// </summary>
    public Room[] rooms;
}