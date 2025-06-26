/// <summary>
/// 场景中方块(障碍物)消息
/// </summary>
public class MsgObstacle : MsgBase
{
    public MsgObstacle()
    {
        protoName = "MsgObstacle";
    }

    public ObstaclePosRotScale[] PosRotScale;
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