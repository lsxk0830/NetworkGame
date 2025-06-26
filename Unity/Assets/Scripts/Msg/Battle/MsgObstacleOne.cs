/// <summary>
/// 场景中一个方块(障碍物)消息
/// </summary>
public class MsgObstacleOne : MsgBase
{
    public MsgObstacleOne()
    {
        protoName = "MsgObstacleOne";
    }

    public ObstaclePosRotScale PosRotScale;

    /// <summary>
    /// 是否销毁
    /// </summary>
    public bool IsDestory = false;
}