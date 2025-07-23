using Unity.Cinemachine;
using UnityEngine;

public class CtrlTank : BaseTank
{
    public float MoveSpeed = 8; // 移动速度
    public float rotateSpeed = 120f;  // 旋转速度（度/秒）

    /// <summary>
    /// 上一次发送同步信息的时间
    /// </summary>
    private float lastSendSyncTime = 0;
    /// <summary>
    /// 同步帧率
    /// </summary>
    public static float syncInterval = 0.1f;

    private float offsetY;
    private CinemachineFreeLook freeLookCam;
    private CinemachineImpulseSource impulseSource;
    private float accumulatedX; // 累计水平旋转角度
    public float maxRayLength = 100f; // 最大射线长度
    public float MouseRotationSpeed = 0.5f; // 鼠标滑动灵敏度系数
    private bool spaceKeyHandled; // 开火标志位
    private string Enemy; // 敌人标签
    private GamePanel gamePanel;

    public override void Init(Player tankInfo)
    {
        freeLookCam = GameObject.FindWithTag("CMFreeLook").GetComponent<CinemachineFreeLook>();
        impulseSource = GameObject.FindWithTag("CMFreeLook").GetComponent<CinemachineImpulseSource>();

        base.Init(tankInfo);

        Enemy = tankInfo.camp == 1 ? "Camp2" : "Camp1";
        freeLookCam.Follow = transform.Find("Follow");
        freeLookCam.LookAt = transform.Find("LookAt");

        // 禁用Cinemachine默认输入
        freeLookCam.m_XAxis.m_InputAxisName = "";
        freeLookCam.m_XAxis.m_AccelTime = 0;

        // 初始化累计角度
        accumulatedX = freeLookCam.m_XAxis.Value;
        // 计算炮塔与相机的初始Y轴偏移
        offsetY = turret.eulerAngles.y - freeLookCam.transform.eulerAngles.y;

        GloablMono.Instance.OnUpdate += OnUpdate;
        GloablMono.Instance.OnFixedUpdate += OnFixUpdate;
        GloablMono.Instance.OnLateUpdate += OnLateUpdate;

        Cursor.lockState = CursorLockMode.Locked;

        gamePanel = PanelManager.Instance.GetPanel<GamePanel>();
    }

    private void OnUpdate()
    {
        if (hp <= 0) return; // 是否死亡
        // 开炮
        FireUpdate();
        // 发送同步信息
        SyncUpdate();
    }
    private void OnFixUpdate()
    {
        if (hp <= 0) return; // 是否死亡
        // 移动控制
        MoveUpdate();
        // 炮塔控制
        TurretUpdate();
    }

    private void OnLateUpdate()
    {
        if (hp <= 0) return; // 是否死亡
        // 更新Cinemachine轴值（立即生效）
        freeLookCam.m_XAxis.Value = accumulatedX;
    }

    private void MoveUpdate()
    {
        // 键盘输入获取
        float moveInput = Input.GetAxis("Vertical");    // W/S 控制前进后退
        float rotateInput = Input.GetAxis("Horizontal"); // A/D 控制左右旋转
        if (moveInput == 0 && rotateInput == 0)
        {
            audioSource.volume = 0; // 停止移动时音量为0
            return; // 无输入则不处理
        }
        if (audioSource.volume == 0)
        {
            audioSource.volume = BattleManager.EffectValue; // 恢复音量
        }

        //Debug.Log($"moveInput:{moveInput},rotateInput:{rotateInput}");
        // 物理移动（基于坦克自身坐标系）
        Vector3 moveDirection = transform.forward * moveInput * MoveSpeed * Time.fixedDeltaTime;
        mRigidbody.MovePosition(mRigidbody.position + moveDirection);

        // 物理旋转（绕Y轴）
        float rotation = rotateInput * rotateSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, rotation, 0);
        mRigidbody.MoveRotation(mRigidbody.rotation * deltaRotation);

        //transform.Translate(transform.forward * Time.fixedDeltaTime * MoveSpeed * moveInput);
    }

    private void TurretUpdate()
    {
        // 获取鼠标位移（已锁定模式，值在[-1,1]区间）
        float mouseX = Input.GetAxis("Mouse X");
        // 直接累加位移量到旋转角度
        accumulatedX += mouseX * MouseRotationSpeed;
        // 同步炮塔Y轴旋转（仅水平方向）
        Vector3 turretEuler = turret.eulerAngles;
        turret.rotation = Quaternion.Euler(turretEuler.x, accumulatedX + offsetY, turretEuler.z);
        //Debug.Log($"炮塔旋转角度: {turretEuler}, 相机旋转角度: {turret.rotation},{turret.rotation.eulerAngles}");
        Draw();
    }

    private void FireUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))  // 按键判断
        {
            if (spaceKeyHandled || Time.time - lastFireTime < fired) return; // CD时间判断

            var startPos = firePoint.transform.position;
            Vector3 targetPos = firePoint.transform.position + firePoint.transform.forward * 50f;
            startPos.y = 1f; targetPos.y = 1f;

            impulseSource.GenerateImpulse(); // 生成震动

            //Debug.Log($"点击开火按钮");
            // 发送同步协议
            MsgFire msg = this.GetObjInstance<MsgFire>();
            msg.ID = GameMain.ID;
            msg.x = startPos.x.RoundTo(4);
            msg.z = startPos.z.RoundTo(4);
            msg.tx = targetPos.x.RoundTo(4);
            msg.tz = targetPos.z.RoundTo(4);
            msg.IsExplosion = false; // 是否爆炸
            NetManager.Instance.Send(msg);
            //Debug.Log($"发送开火协议：坐标 ={firePoint.position}, 目标 ={bullet.targetPos}");
            this.PushPool(msg); // 将消息对象归还对象池
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
        MsgSyncTank msg = this.GetObjInstance<MsgSyncTank>();
        msg.x = transform.position.x.RoundTo(4);
        msg.y = transform.position.y.RoundTo(4);
        msg.z = transform.position.z.RoundTo(4);
        msg.ex = transform.eulerAngles.x.RoundTo(4);
        msg.ey = transform.eulerAngles.y.RoundTo(4);
        msg.ez = transform.eulerAngles.z.RoundTo(4);
        msg.turretY = turret.localEulerAngles.y.RoundTo(4);
        this.PushPool(msg); // 将消息对象归还对象池
        NetManager.Instance.Send(msg);
    }

    /// <summary>
    /// 绘制射线
    /// </summary>
    private void Draw()
    {
        // 1. 计算射线方向（炮管正前方）
        Vector3 rayStart = firePoint.position; rayStart.y = 0.8f;
        Vector3 rayDirection = firePoint.forward;

        // 2. 发射射线检测碰撞
        RaycastHit hit;
        bool isHit = Physics.Raycast(rayStart, rayDirection, out hit, maxRayLength);
        gamePanel.FrontSight.transform.position = Camera.main.WorldToScreenPoint(hit.point);
        // 3. 根据碰撞标签切换颜色
        if (isHit)
        {
            if (hit.collider.CompareTag(Enemy))
            {
                gamePanel.FrontSight.color = Color.red; // 敌人：红色
            }
            else if (hit.collider.CompareTag("Obstacle"))
            {
                gamePanel.FrontSight.color = Color.green; // 障碍物：白色
            }
            else
            {
                gamePanel.FrontSight.color = Color.white; // 其他情况：白色
            }
        }

    }

    private void OnDestroy()
    {
        Destroy();
    }

    protected override void Destroy()
    {
        base.Destroy();
        Cursor.lockState = CursorLockMode.None;
        GloablMono.Instance.OnUpdate -= OnUpdate;
        GloablMono.Instance.OnFixedUpdate -= OnFixUpdate;
        GloablMono.Instance.OnLateUpdate -= OnLateUpdate;
        freeLookCam = null;
        impulseSource = null;
    }
}