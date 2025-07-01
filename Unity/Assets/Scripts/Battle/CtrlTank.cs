using System;
using Cinemachine;
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
    private float accumulatedX; // 累计水平旋转角度
    private LineRenderer lineRenderer; // 划线
    public Material mMaterial; // 射线材质
    public float maxRayLength = 100f; // 最大射线长度
    public float MouseRotationSpeed = 0.5f; // 鼠标滑动灵敏度系数
    private bool spaceKeyHandled; // 开火标志位
    private string Enemy;

    public override void Init(Player tankInfo)
    {
        freeLookCam = BattleManager.Instance.GetComponent<CinemachineFreeLook>();

        base.Init(tankInfo);

        Enemy = tankInfo.camp == 1 ? "Camp2" : "Camp1";

        freeLookCam.Follow = transform.Find("Follow");
        freeLookCam.LookAt = transform.Find("LookAt");

        // 禁用Cinemachine默认输入
        freeLookCam.m_XAxis.m_InputAxisName = "";
        freeLookCam.m_YAxis.m_InputAxisName = "";
        freeLookCam.m_XAxis.m_AccelTime = 0;
        freeLookCam.m_YAxis.m_AccelTime = 0;

        // 初始化累计角度
        accumulatedX = freeLookCam.m_XAxis.Value;
        // 计算炮塔与相机的初始Y轴偏移
        offsetY = turret.eulerAngles.y - freeLookCam.transform.eulerAngles.y;

        // 初始化LineRenderer组件
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;

        mMaterial = new Material(Shader.Find("Standard"));
        mMaterial.color = Color.green;
        lineRenderer.material = mMaterial;

        GloablMono.Instance.OnUpdate += OnUpdate;
        GloablMono.Instance.OnFixedUpdate += OnFixUpdate;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;// 隐藏鼠标并锁定到屏幕中心
        Cursor.lockState = CursorLockMode.Confined;//鼠标限制在Game视图
    }

    private void OnUpdate()
    {
        // 开炮
        FireUpdate();
        // 发送同步信息
        SyncUpdate();
    }
    private void OnFixUpdate()
    {
        // 移动控制
        MoveUpdate();
        // 炮塔控制
        TurretUpdate();
    }

    private void MoveUpdate()
    {
        // 键盘输入获取
        float moveInput = Input.GetAxis("Vertical");    // W/S 控制前进后退
        float rotateInput = Input.GetAxis("Horizontal"); // A/D 控制左右旋转
        //Debug.Log($"moveInput:{moveInput},rotateInput:{rotateInput}");
        // 物理移动（基于坦克自身坐标系）
        Vector3 moveDirection = transform.forward * moveInput * MoveSpeed * Time.fixedDeltaTime;
        mRigidbody.MovePosition(mRigidbody.position + moveDirection);

        // 物理旋转（绕Y轴）
        float rotation = rotateInput * rotateSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, rotation, 0);
        mRigidbody.MoveRotation(mRigidbody.rotation * deltaRotation);
    }

    private void TurretUpdate()
    {
        // 获取鼠标位移（已锁定模式，值在[-1,1]区间）
        float mouseX = Input.GetAxis("Mouse X");
        // 直接累加位移量到旋转角度
        accumulatedX += mouseX * MouseRotationSpeed;
        // 更新Cinemachine轴值（立即生效）
        freeLookCam.m_XAxis.Value = accumulatedX;
        // 同步炮塔Y轴旋转（仅水平方向）
        Vector3 turretEuler = turret.eulerAngles;
        turret.rotation = Quaternion.Euler(turretEuler.x, accumulatedX + offsetY, turretEuler.z);
        Draw();
    }

    private void FireUpdate()
    {
        if (isDie()) return; // 是否死亡

        if (Input.GetKeyDown(KeyCode.Space))  // 按键判断
        {
            if (spaceKeyHandled || Time.time - lastFireTime < fired) return; // CD时间判断

            Bullet bullet = Fire(Guid.NewGuid());
            Debug.Log($"点击开火按钮");
            // 发送同步协议
            MsgFire msg = this.GetObjInstance<MsgFire>();
            msg.ID = GameMain.ID;
            msg.bulletID = bullet.bulletID;
            msg.x = firePoint.position.x;
            msg.y = firePoint.position.y;
            msg.z = firePoint.position.z;
            msg.tx = bullet.targetPos.x;
            msg.ty = bullet.targetPos.y;
            msg.tz = bullet.targetPos.z;
            NetManager.Instance.Send(msg);
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
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = transform.eulerAngles.x;
        msg.ey = transform.eulerAngles.y;
        msg.ez = transform.eulerAngles.z;
        msg.turretY = turret.localEulerAngles.y;
        this.PushPool(msg); // 将消息对象归还对象池
        NetManager.Instance.Send(msg);
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GloablMono.Instance.OnUpdate -= OnUpdate;
        GloablMono.Instance.OnFixedUpdate -= OnFixUpdate;
    }

    /// <summary>
    /// 绘制射线
    /// </summary>
    private void Draw()
    {
        // 1. 计算射线方向（炮管正前方）
        Vector3 rayStart = firePoint.position;
        Vector3 rayDirection = firePoint.forward;

        // 2. 发射射线检测碰撞
        RaycastHit hit;
        bool isHit = Physics.Raycast(rayStart, rayDirection, out hit, maxRayLength);
        //Debug.DrawRay(rayStart, rayDirection * 10, Color.green);

        // 3. 动态设置线段终点
        Vector3 lineEnd = isHit ? hit.point : rayStart + rayDirection * maxRayLength;
        lineRenderer.SetPosition(0, rayStart); // 起点：炮管末端
        lineRenderer.SetPosition(1, lineEnd); // 终点：碰撞点或最大长度点

        // 4. 根据碰撞标签切换线段颜色
        if (isHit)
        {
            if (hit.collider.CompareTag(Enemy))
            {
                if (mMaterial.color != Color.red)
                    mMaterial.color = Color.red; // 击中敌人：红色
            }
            else if (hit.collider.CompareTag("Obstacle"))
            {
                if (mMaterial.color != Color.yellow)
                    mMaterial.color = Color.yellow; // 其他情况：绿色
            }
            else
            {
                if (mMaterial.color != Color.green)
                    mMaterial.color = Color.green; // 其他情况：绿色
            }
        }

    }
}