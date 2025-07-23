/// <summary>
/// 开始战斗
/// </summary>
public class MsgStartBattle : MsgBase
{
    public MsgStartBattle()
    {
        protoName = "MsgStartBattle";
    }

    /// <summary>
    /// 服务器返回的战斗结果 0-成功 其他数值-失败
    /// </summary>
    public int result { get; set; } = 0;

    /// <summary>
    /// 战斗开始的房间
    /// </summary>
    public string roomID { get; set; }

    /// <summary>
    /// 地图大小
    /// </summary>
    public int mapSize { get; set; }

    /// <summary>
    /// 障碍物数量
    /// </summary>
    public int obstacleCount { get; set; }
}