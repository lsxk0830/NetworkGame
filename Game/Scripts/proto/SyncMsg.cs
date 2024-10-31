// 同步协议、同步坦克信息、开火、击中

/// <summary>
/// 同步协议
/// </summary>
public class MsgFrameSync : MsgBase
{
    public MsgFrameSync() { protoName = "MsgFrameSync"; }

    /// <summary>
    /// 指令，0-前进 1-后退 2-左转 3-右转 4-停止
    /// </summary>
    public int cmd { get; set; } = 0;

    /// <summary>
    /// 在第几帧发生事件
    /// </summary>
    public int frame { get; set; } = 0;
}

/// <summary>
/// 同步坦克信息
/// </summary>
public class MsgSyncTank : MsgBase
{
    public MsgSyncTank() { protoName = "MsgStartBattle"; }

    //位置
    public float x { get; set; } = 0;
    public float y { get; set; } = 0;
    public float z { get; set; } = 0;
    //旋转
    public float ex { get; set; } = 0;
    public float ey { get; set; } = 0;
    public float ez { get; set; } = 0;

    /// <summary>
    /// 炮塔旋转.y
    /// </summary>
    public float turretY { get; set; } = 0;

    /// <summary>
    /// 服务端补充，哪个坦克
    /// </summary>
    public string id { get; set; } = "";

}

/// <summary>
/// 开火
/// </summary>
public class MsgFire : MsgBase
{
    public MsgFire() { protoName = "MsgFire"; }

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
    public string id { get; set; } = "";
}

/// <summary>
/// 击中
/// </summary>
public class MsgHit : MsgBase
{
    public MsgHit() { protoName = "MsgHit"; }

    //击中点
    public float x { get; set; } = 0;
    public float y { get; set; } = 0;
    public float z { get; set; } = 0;

    /// <summary>
    /// 服务端补充，击中谁
    /// </summary>
    public string targetId { get; set; } = "";

    /// <summary>
    /// 服务端补充，哪个坦克
    /// </summary>
    public string id { get; set; } = "";

    /// <summary>
    /// 服务端补充，被击中坦克血量
    /// </summary>
    public int hp { get; set; } = 0;

    /// <summary>
    /// 服务端补充，受到的伤害
    /// </summary>
    public int damage { get; set; } = 0;
}