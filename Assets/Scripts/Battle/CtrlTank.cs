using System;
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
    private bool spaceKeyHandled; // 开火标志位

    [LabelText("射线检测的层级")][SerializeField] private LayerMask RayLayers;

    public override void Init(Player tankInfo)
    {
        RayLayers = LayerMask.GetMask("Default", "Enemy", "Friend", "CanDestroy");

        freeLookCam = GameObject.FindWithTag("CMFreeLook").GetComponent<CinemachineCamera>();
        impulseSource = GameObject.FindWithTag("CMFreeLook").GetComponent<CinemachineImpulseSource>();
        cinemachineOrbitalFollow = GameObject.FindWithTag("CMFreeLook").GetComponent<CinemachineOrbitalFollow>();

        base.Init(tankInfo);

        freeLookCam.Follow = transform;

        offsetY = turret.parent.localEulerAngles.y; // 模型制作的问题，导致特殊对待
        GloablMono.Instance.OnUpdate += OnUpdate;
        GloablMono.Instance.OnFixedUpdate += OnFixUpdate;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        //Draw();
    }

    private void FireUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (spaceKeyHandled || Time.time - lastFireTime < fired) return; // CD时间判断
            spaceKeyHandled = true;

            MsgAttack msg = this.GetObjInstance<MsgAttack>();
            msg.ID = GameMain.ID;
            msg.x = (int)Math.Round(firePoint.transform.position.x * BattleManager.Scale);
            msg.y = (int)Math.Round(firePoint.transform.position.y * BattleManager.Scale);
            msg.z = (int)Math.Round(firePoint.transform.position.z * BattleManager.Scale);
            // 发射射线检测碰撞
            if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, maxRayLength, RayLayers))
            {
                if (hit.collider.TryGetComponent<SyncTank>(out SyncTank syncTank))
                {
                    msg.hitID = syncTank.ID;
                }
                else
                {
                    msg.hitID = -1;
                }
                msg.fx = (int)Math.Round(firePoint.forward.x * BattleManager.Scale);
                msg.fy = (int)Math.Round(firePoint.forward.y * BattleManager.Scale);
                msg.fz = (int)Math.Round(firePoint.forward.z * BattleManager.Scale);
                msg.tx = (int)Math.Round(hit.point.x * BattleManager.Scale);
                msg.ty = (int)Math.Round(hit.point.y * BattleManager.Scale);
                msg.tz = (int)Math.Round(hit.point.z * BattleManager.Scale);
                Debug.DrawRay(firePoint.position, firePoint.forward * 500, Color.green, 100f);
            }
            else
            {
                msg.hitID = -1;
                msg.fx = 0;
                msg.fy = 0;
                msg.fz = 0;
                msg.tx = 0;
                msg.ty = 0;
                msg.tz = 0;
            }
            //Debug.LogError($"发送开火协议.ID:{msg.hitID}：坐标 ={firePoint.transform.position},方向：{firePoint.forward}, 目标 ={hit.point}");
            NetManager.Instance.Send(msg);
            Fire();
            impulseSource.GenerateImpulse(); // 生成震动
        }
        if (Input.GetMouseButtonUp(1))
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
        msg.x = (int)(transform.position.x * BattleManager.Scale);
        msg.y = (int)(transform.position.y * BattleManager.Scale);
        msg.z = (int)(transform.position.z * BattleManager.Scale);
        msg.ex = (int)(transform.eulerAngles.x * BattleManager.Scale);
        msg.ey = (int)(transform.eulerAngles.y * BattleManager.Scale);
        msg.ez = (int)(transform.eulerAngles.z * BattleManager.Scale);
        msg.turretY = (int)(turret.localEulerAngles.y * BattleManager.Scale);
        this.PushPool(msg);
        NetManager.Instance.Send(msg);
    }

    private void OnDestroy()
    {
        base.Destroy();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GloablMono.Instance.OnUpdate -= OnUpdate;
        GloablMono.Instance.OnFixedUpdate -= OnFixUpdate;
        freeLookCam = null;
        impulseSource = null;
    }
}