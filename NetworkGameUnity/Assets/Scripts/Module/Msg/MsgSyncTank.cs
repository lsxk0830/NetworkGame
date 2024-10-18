/// <summary>
/// 同步坦克信息
/// </summary>
public class MsgSyncTank : MsgBase
{
    public MsgSyncTank()
    {
        protoName = "MsgStartBattle";
    }

    /// <summary>
    /// 位置.x
    /// </summary>
    public float x = 0;

    /// <summary>
    /// 位置.y
    /// </summary>
    public float y = 0;

    /// <summary>
    /// 位置.z
    /// </summary>
    public float z = 0;

    /// <summary>
    /// 旋转.ex
    /// </summary>
    public float ex = 0;

    /// <summary>
    /// 旋转.ey
    /// </summary>
    public float ey = 0;

    /// <summary>
    /// 旋转.ez
    /// </summary>
    public float ez = 0;

    /// <summary>
    /// 炮塔旋转.y
    /// </summary>
    public float turretY = 0;

    /// <summary>
    /// 服务端补充，哪个坦克
    /// </summary>
    public string id = "";

}