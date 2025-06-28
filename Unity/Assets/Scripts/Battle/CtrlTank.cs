using UnityEngine;

public class CtrlTank : BaseTank
{
    public float Speed = 6; // 移动速度
    public Vector3 currentVelocity; // 当前速度（用于平滑
    public float VelocityDamping = 5; // 速度阻尼
    public bool RotatePlayer = true;
    /// <summary>
    /// 上一次发送同步信息的时间
    /// </summary>
    private float lastSendSyncTime = 0;
    /// <summary>
    /// 同步帧率
    /// </summary>
    public static float syncInterval = 0.1f;

    private bool spaceKeyHandled; // 添加状态标志位

    public override void Init(Player tankInfo)
    {
        base.Init(tankInfo);
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    private void OnUpdate()
    {
        // 移动控制
        //MoveUpdate();
        // 炮塔控制
        //TurretUpdate();
        // 开炮
        FireUpdate();
        // 发送同步信息
        SyncUpdate();
    }

    private void FireUpdate()
    {
        if (isDie()) return; // 是否死亡

        if (Input.GetKeyDown(KeyCode.Space))  // 按键判断
        {
            if (spaceKeyHandled || Time.time - lastFireTime < fired) return; // CD时间判断

            Bullet bullet = Fire();
            Debug.Log($"点击开火按钮");
            // 发送同步协议
            MsgFire msg = new MsgFire();
            msg.ID = GameMain.ID;
            msg.bulletID = bullet.bulletID;
            msg.x = bullet.transform.position.x;
            msg.y = bullet.transform.position.y;
            msg.z = bullet.transform.position.z;
            msg.ex = bullet.transform.eulerAngles.x;
            msg.ey = bullet.transform.eulerAngles.y;
            msg.ez = bullet.transform.eulerAngles.z;
            NetManager.Send(msg);
            spaceKeyHandled = true;
        }
        else
        {
            spaceKeyHandled = false;
        }

    }

    private void SyncUpdate()
    {
        // 时间间隔判断
        if (Time.time - lastSendSyncTime < syncInterval) return;
        lastSendSyncTime = Time.time;
        // 发送同步协议
        MsgSyncTank msg = new MsgSyncTank();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = transform.eulerAngles.x;
        msg.ey = transform.eulerAngles.y;
        msg.ez = transform.eulerAngles.z;
        msg.turretY = turret.localEulerAngles.y;
        NetManager.Send(msg);
    }

    private void OnDestroy()
    {
        GloablMono.Instance.OnUpdate -= OnUpdate;
    }
}