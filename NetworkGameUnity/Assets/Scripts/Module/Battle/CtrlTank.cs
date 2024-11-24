using UnityEngine;

public class CtrlTank : BaseTank
{
    /// <summary>
    /// 上一次发送同步信息的时间
    /// </summary>
    private float lastSendSyncTime = 0;
    /// <summary>
    /// 同步帧率
    /// </summary>
    public static float syncInterval = 0.1f;

    private void Start()
    {
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    private void OnUpdate(float f)
    {
        // 移动控制
        MoveUpdate();
        // 炮塔控制
        TurretUpdate();
        // 开炮
        FireUpdate();
        // 发送同步信息
        SyncUpdate();
    }

    private void MoveUpdate()
    {
        if (isDie()) return;

        float x = Input.GetAxis("Horizontal");
        transform.Rotate(0, x * steer * Time.deltaTime, 0);
        float y = Input.GetAxis("Vertical");
        Vector3 s = y * transform.forward * speed * Time.deltaTime;
        transform.position += s;
    }

    private void TurretUpdate()
    {
        if (isDie()) return;

        float axis = 0;
        if (Input.GetKey(KeyCode.Q))
            axis = -1;
        else if (Input.GetKey(KeyCode.E))
            axis = 1;
        // 旋转角度
        Vector3 le = turret.localEulerAngles;
        le.y += axis * Time.deltaTime * turretSpeed;
        turret.localEulerAngles = le;
    }

    private void FireUpdate()
    {
        if (isDie()) return; // 是否死亡

        if (!Input.GetKeyDown(KeyCode.Space)) return; // 按键判断

        if (Time.time - lastFireTime < fired) return; // CD时间判断

        Bullet bullet = Fire();
        // 发送同步协议
        MsgFire msg = new MsgFire();
        msg.x = bullet.transform.position.x;
        msg.y = bullet.transform.position.y;
        msg.z = bullet.transform.position.z;
        msg.ex = bullet.transform.eulerAngles.x;
        msg.ey = bullet.transform.eulerAngles.y;
        msg.ez = bullet.transform.eulerAngles.z;
        NetManager.Send(msg);
    }

    private void SyncUpdate()
    {
        // 时间间隔判断
        if (Time.time - lastSendSyncTime < syncInterval)
            return;
        lastSendSyncTime = Time.time;
        // 发送同步协议
        MsgSyncTank msg = new MsgSyncTank();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.x = transform.eulerAngles.x;
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