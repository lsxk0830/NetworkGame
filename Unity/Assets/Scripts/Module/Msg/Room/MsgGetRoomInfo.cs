/// <summary>
/// 获取房间玩家信息
/// </summary>
public class MsgGetRoomInfo : MsgBase
{
    public MsgGetRoomInfo() { protoName = "MsgGetRoomInfo"; }

    /// <summary>
    /// 房间的玩家信息
    /// </summary>
    public Player[] Players;
}