/// <summary>
/// 击中
/// </summary>
public class MsgHit : MsgBase
{
    public MsgHit()
    {
        protoName = "MsgHit";
    }

    //击中点
    public float x { get; set; } = 0;

    public float y { get; set; } = 0;
    public float z { get; set; } = 0;

    /// <summary>
    /// 击中谁
    /// </summary>
    public long targetId { get; set; }

    /// <summary>
    /// 哪个坦克打的
    /// </summary>
    public long id { get; set; }

    /// <summary>
    /// 服务端补充，被击中坦克血量
    /// </summary>
    public int hp { get; set; } = 0;

    /// <summary>
    /// 服务端补充，受到的伤害
    /// </summary>
    public int damage { get; set; } = 0;
}