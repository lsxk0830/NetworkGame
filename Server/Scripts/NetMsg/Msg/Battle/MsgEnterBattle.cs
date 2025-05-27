/// <summary>
/// 进入战场（服务器推送）
/// </summary>
public class MsgEnterBattle : MsgBase
{
    public MsgEnterBattle()
    {
        protoName = "MsgEnterBattle";
    }

    /// <summary>
    /// 服务器返回的坦克列表信息
    /// </summary>
    public TankInfo[] tanks { get; set; }

    /// <summary>
    /// 地图，只有一张
    /// </summary>
    public int mapId { get; set; } = 1;
}