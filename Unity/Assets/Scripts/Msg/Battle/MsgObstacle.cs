/// <summary>
/// 场景中方块(障碍物)消息
/// </summary>
public class MsgObstacle : MsgBase
{
    public MsgObstacle()
    {
        protoName = "MsgObstacle";
    }

    /// <summary>
    /// -1为第一次发送消息
    /// </summary>
    public int result = -1;

    public ObstaclePosRotScale[] PosRotScales;
}