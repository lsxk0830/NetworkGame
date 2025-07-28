/// <summary>
/// 击中
/// </summary>
public class MsgAttack : MsgBase
{
    public MsgAttack()
    {
        protoName = "MsgAttack";
    }

    //炮弹初始位置
    public float x = 0;
    public float y = 0;
    public float z = 0;

    //炮弹方向
    public float fx = 0;
    public float fy = 0;
    public float fz = 0;

    //击中点位置
    public float tx = 0;
    public float ty = 0;
    public float tz = 0;

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