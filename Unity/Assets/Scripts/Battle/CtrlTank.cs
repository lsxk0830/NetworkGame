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

    public override void Init(Player tankInfo)
    {
        base.Init(tankInfo);
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    private void Update()
    {
        MoveUpdate();

    }
    private void OnUpdate()
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

        Vector3 fwd = transform.forward;
        fwd.y = 0;
        if (fwd.sqrMagnitude < 0.01f) return;

        // 1. 获取输入并转换到角色朝向空间
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input = Quaternion.LookRotation(fwd) * input;

        // 2. 直接平滑速度（最简单的阻尼实现）
        currentVelocity = Vector3.Lerp(currentVelocity, input * Speed, VelocityDamping * Time.deltaTime);

        // 3. 移动角色
        transform.position += currentVelocity * Time.deltaTime;

        // 4. 简单旋转（仅当有水平输入或前进时旋转）
        if (RotatePlayer && currentVelocity.sqrMagnitude > 0.01f)
        {
            // 仅当速度方向与当前朝向大致一致时才旋转（避免后退时旋转）
            if (Vector3.Dot(currentVelocity.normalized, fwd) > 0.2f || Mathf.Abs(input.x) > 0.1f)
            {
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.LookRotation(currentVelocity.normalized),
                    VelocityDamping * Time.deltaTime
                );
            }
        }
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