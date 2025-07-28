/// <summary>
/// 攻击协议
/// </summary>
public class MsgAttack : MsgBase
{
    public MsgAttack()
    {
        protoName = "MsgAttack";
    }

    //炮弹初始位置
    public int x = 0;
    public int y = 0;
    public int z = 0;

    //炮弹方向
    public int fx = 0;
    public int fy = 0;
    public int fz = 0;

    //击中点位置
    public int tx = 0;
    public int ty = 0;
    public int tz = 0;

    /// <summary>
    /// 哪个坦克开火的
    /// </summary>
    public long ID;

    /// <summary>
    /// 哪个坦克受伤
    /// </summary>
    public long hitID;

    /// <summary>
    /// 网络判断是否受伤
    /// </summary>
    public bool isHit;

    /// <summary>
    /// 被击中坦克血量
    /// </summary>
    public int hp = 0;

    /// <summary>
    /// 受到的伤害
    /// </summary>
    public int damage = 0;
}