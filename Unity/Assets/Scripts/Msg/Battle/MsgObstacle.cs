/// <summary>
/// 场景中方块(障碍物)消息
/// </summary>
public class MsgObstacle : MsgBase
{
    public MsgObstacle()
    {
        protoName = "MsgObstacle";
    }

    public ObstaclePosRotScale[] PosRotScales;
}