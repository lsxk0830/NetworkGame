/// <summary>
/// 场景中所有方块(障碍物)消息
/// </summary>
public class MsgObstacleAll : MsgBase
{
    public MsgObstacleAll()
    {
        protoName = "MsgObstacleAll";
    }

    public List<ObstaclePosRotScale> PosRotScales { get; set; }
}