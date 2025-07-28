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
    public int x = 0;
    public int y = 0;
    public int z = 0;

    //旋转
    public int ex = 0;
    public int ey = 0;
    public int ez = 0;

    /// <summary>
    /// 炮塔旋转.y
    /// </summary>
    public int turretY = 0;

    /// <summary>
    /// 服务端补充，哪个坦克
    /// </summary>
    public long ID;
}