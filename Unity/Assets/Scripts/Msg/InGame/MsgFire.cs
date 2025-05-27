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
    public float x = 0;
    public float y = 0;
    public float z = 0;

    //炮弹初始旋转
    public float ex = 0;
    public float ey = 0;
    public float ez = 0;

    /// <summary>
    /// 服务端补充，哪个坦克
    /// </summary>
    public long ID;
}