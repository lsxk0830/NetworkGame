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

    //public float y { get; set; } = 1;
    public float z { get; set; } = 0;

    //炮弹目标位置
    public float tx { get; set; } = 0;

    //public float ty { get; set; } = 1;
    public float tz { get; set; } = 0;

    /// <summary>
    /// 哪个坦克开火的
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// 是否爆炸
    /// </summary>
    public bool IsExplosion { get; set; } = false;

    /// <summary>
    /// 子弹ID,用于区别哪个子弹发生了爆炸
    /// </summary>
    public Guid bulletID { get; set; }
}