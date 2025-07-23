/// <summary>
/// 场景中一个方块(障碍物)消息
/// </summary>
public class MsgObstacleOne : MsgBase
{
    public MsgObstacleOne()
    {
        protoName = "MsgObstacleOne";
    }

    /// <summary>
    /// 障碍物ID
    /// </summary>
    public int ObstacleID { get; set; }

    public ObstaclePosRotScale PosRotScale { get; set; }

    /// <summary>
    /// 是否销毁
    /// </summary>
    public bool IsDestory { get; set; } = false;
}