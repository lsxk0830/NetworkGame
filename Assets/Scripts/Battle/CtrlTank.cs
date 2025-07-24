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
    private CinemachineCamera freeLookCam;
    private CinemachineImpulseSource impulseSource;
    private CinemachineOrbitalFollow cinemachineOrbitalFollow;
    private float maxRayLength = 300f; // 最大射线长度
    private float MouseRotationSpeed = 0.5f; // 鼠标滑动灵敏度系数
    private bool spaceKeyHandled; // 开火标志位
    private string Enemy; // 敌人标签
    private GamePanel gamePanel;

    public override void Init(Player tankInfo)
    {
        freeLookCam = GameObject.FindWithTag("CMFreeLook").GetComponent<CinemachineCamera>();
        impulseSource = GameObject.FindWithTag("CMFreeLook").GetComponent<CinemachineImpulseSource>();
        cinemachineOrbitalFollow = GameObject.FindWithTag("CMFreeLook").GetComponent<CinemachineOrbitalFollow>();

        base.Init(tankInfo);

        Enemy = tankInfo.camp == 1 ? "Camp2" : "Camp1";
        freeLookCam.Follow = transform;

        offsetY = turret.parent.localEulerAngles.y; // 模型制作的问题，导致特殊对待
        GloablMono.Instance.OnUpdate += OnUpdate;
        GloablMono.Instance.OnFixedUpdate += OnFixUpdate;

        Cursor.lockState = CursorLockMode.Locked;

        gamePanel = PanelManager.Instance.GetPanel<GamePanel>();
    }

    private void OnUpdate()
    {
        if (hp <= 0) return; // 是否死亡
        // 开炮
        FireUpdate();
        // 炮塔控制
        TurretUpdate();
        // 发送同步信息
        SyncUpdate();
    }
    private void OnFixUpdate()
    {
        if (hp <= 0) return; // 是否死亡
        // 移动控制
        MoveUpdate();
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
        Quaternion rotation = Quaternion.Euler(0, cinemachineOrbitalFollow.HorizontalAxis.Value + offsetY, 0);
        turret.rotation = rotation;
        //Debug.Log($"炮塔旋转角度: {turretEuler}, 相机旋转角度: {turret.rotation},{turret.rotation.eulerAngles}");
        Draw();
    }

    private void FireUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))  // 按键判断
        {
            if (spaceKeyHandled || Time.time - lastFireTime < fired) return; // CD时间判断
            spaceKeyHandled = true;
            Vector3 targetPos = firePoint.transform.position + firePoint.transform.forward * 50f;

            impulseSource.GenerateImpulse(); // 生成震动

            //Debug.Log($"点击开火按钮");
            // 发送同步协议
            MsgFire msg = this.GetObjInstance<MsgFire>();
            msg.ID = GameMain.ID;
            msg.x = firePoint.transform.position.x.RoundTo(4);
            msg.y = firePoint.transform.position.y.RoundTo(4);
            msg.z = firePoint.transform.position.z.RoundTo(4);
            msg.tx = targetPos.x.RoundTo(4);
            msg.ty = targetPos.y.RoundTo(4);
            msg.tz = targetPos.z.RoundTo(4);
            msg.IsExplosion = false; // 是否爆炸
            NetManager.Instance.Send(msg);
            //Debug.LogError($"发送开火协议：坐标 ={firePoint.transform.position}, 目标 ={targetPos}");
            this.PushPool(msg); // 将消息对象归还对象池
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            spaceKeyHandled = false; // 释放按键
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
        freeLookCam = null;
        impulseSource = null;
    }
}