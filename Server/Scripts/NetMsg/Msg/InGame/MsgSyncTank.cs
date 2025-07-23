/// <summary>
/// 同步坦克信息
/// </summary>
public class MsgSyncTank : MsgBase
{
    public MsgSyncTank()
    {
        protoName = "MsgStartBattle";
    }

    //位置
    public float x { get; set; } = 0;

    public float y { get; set; } = 0;
    public float z { get; set; } = 0;

    //旋转
    public float ex { get; set; } = 0;

    public float ey { get; set; } = 0;
    public float ez { get; set; } = 0;

    /// <summary>
    /// 炮塔旋转.y
    /// </summary>
    public float turretY { get; set; } = 0;

    /// <summary>
    /// 服务端补充，哪个坦克
    /// </summary>
    public long id { get; set; }
}