// 进入战场、战斗结果、玩家退出

/// <summary>
/// 进入战场（服务器推送）
/// </summary>
public class MsgEnterBattle : MsgBase
{
    public MsgEnterBattle()
    { protoName = "MsgEnterBattle"; }

    /// <summary>
    /// 服务器返回的坦克列表信息
    /// </summary>
    public TankInfo[] tanks { get; set; }

    /// <summary>
    /// 地图，只有一张
    /// </summary>
    public int mapId { get; set; } = 1;
}

/// <summary>
/// 战斗结果（服务器推送）
/// </summary>
public class MsgBattleResult : MsgBase
{
    public MsgBattleResult()
    { protoName = "MsgBattleResult"; }

    /// <summary>
    /// 获胜的阵营
    /// </summary>
    public int winCamp { get; set; } = 0;
}

/// <summary>
/// 玩家退出（服务器推送）
/// </summary>
public class MsgLeaveBattle : MsgBase
{
    public MsgLeaveBattle()
    { protoName = "MsgLeaveBattle"; }

    /// <summary>
    /// 服务器返回的玩家Id
    /// </summary>
    public long id { get; set; }
}