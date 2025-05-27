/// <summary>
/// 同步坦克信息
/// </summary>
public class MsgSyncTank : MsgBase
{
    public MsgSyncTank()
    {
        protoName = "MsgSyncTank";
    }

    //位置
    public float x = 0;
    public float y = 0;
    public float z = 0;

    //旋转
    public float ex = 0;
    public float ey = 0;
    public float ez = 0;

    /// <summary>
    /// 炮塔旋转.y
    /// </summary>
    public float turretY = 0;

    /// <summary>
    /// 服务端补充，哪个坦克
    /// </summary>
    public long ID;
}