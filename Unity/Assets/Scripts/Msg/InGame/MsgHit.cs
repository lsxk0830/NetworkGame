/// <summary>
/// 击中
/// </summary>
public class MsgHit : MsgBase
{
    public MsgHit()
    {
        protoName = "MsgHit";
    }

    /// <summary>
    /// 击中谁
    /// </summary>
    public long targetID;

    /// <summary>
    /// 哪个坦克攻击的
    /// </summary>
    public long ID;

    //击中点
    public float x = 0;
    public float y = 0;
    public float z = 0;

    /// <summary>
    /// 被击中坦克血量
    /// </summary>
    public int hp = 0;

    /// <summary>
    /// 受到的伤害
    /// </summary>
    public int damage = 0;
}