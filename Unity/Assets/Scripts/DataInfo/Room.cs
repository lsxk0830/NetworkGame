using System;
using System.Collections.Generic;

/// <summary>
/// 房间信息
/// </summary>
[Serializable]
public class Room
{
    /// <summary>
    /// 房间ID
    /// </summary>
    public string RoomID;

    /// <summary>
    /// 玩家列表
    /// </summary>
    public Dictionary<long, Player> playerIds;

    /// <summary>
    /// 房主id
    /// </summary>
    public long ownerId = -1;

    /// <summary>
    /// 状态 0-准备中 1-战斗中
    /// </summary>
    public int status = 0;
}