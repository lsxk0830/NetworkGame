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

    public List<ObstaclePosRotScale> PosRotScale;

    /// <summary>
    /// 销毁的障碍物ID
    /// </summary>
    public string destoryID;
}

[Serializable]
public struct ObstaclePosRotScale
{
    /// <summary>
    /// 障碍物ID
    /// </summary>
    public string ObstacleID;

    public float PosX;
    public float PosY;
    public float PosZ;

    public float RotX;
    public float RotY;
    public float RotZ;

    public float ScaleX;
    public float ScaleY;
    public float ScaleZ;
}