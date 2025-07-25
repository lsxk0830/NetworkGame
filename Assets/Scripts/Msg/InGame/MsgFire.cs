using System;

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

    /// <summary>
    /// 哪个坦克开火的
    /// </summary>
    public long ID;

    /// <summary>
    /// 是否爆炸
    /// </summary>
    public bool IsExplosion = false;

    /// <summary>
    /// 子弹ID,用于区别哪个子弹发生了爆炸
    /// </summary>
    public Guid bulletID;
}