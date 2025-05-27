/// <summary>
/// 开火
/// </summary>
public class MsgFire : MsgBase
{
    public MsgFire()
    {
        protoName = "MsgFire";
    }

    //炮弹初始位置
    public float x { get; set; } = 0;

    public float y { get; set; } = 0;
    public float z { get; set; } = 0;

    //炮弹初始旋转
    public float ex { get; set; } = 0;

    public float ey { get; set; } = 0;
    public float ez { get; set; } = 0;

    /// <summary>
    /// 服务端补充，哪个坦克
    /// </summary>
    public long id { get; set; }
}