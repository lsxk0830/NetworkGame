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
    public int cmd = 0;

    /// <summary>
    /// 在第几帧发生事件
    /// </summary>
    public int frame = 0;
}

/// <summary>
/// 同步坦克信息
/// </summary>
public class MsgSyncTank : MsgBase
{
    public MsgSyncTank() { protoName = "MsgStartBattle"; }

    //位置
    public float x = 0;
    public float y = 0;
    public float z = 0;
    //旋转
    public float ex = 0;
    public float ey = 0;
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

/// <summary>
/// 开火
/// </summary>
public class MsgFire : MsgBase
{
    public MsgFire() { protoName = "MsgFire"; }

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
    public string id = "";
}

/// <summary>
/// 击中
/// </summary>
public class MsgHit : MsgBase
{
    public MsgHit() { protoName = "MsgHit"; }

    //击中点
    public float x = 0;
    public float y = 0;
    public float z = 0;

    /// <summary>
    /// 服务端补充，击中谁
    /// </summary>
    public string targetId = "";

    /// <summary>
    /// 服务端补充，哪个坦克
    /// </summary>
    public string id = "";

    /// <summary>
    /// 服务端补充，被击中坦克血量
    /// </summary>
    public int hp = 0;

    /// <summary>
    /// 服务端补充，受到的伤害
    /// </summary>
    public int damage = 0;
}