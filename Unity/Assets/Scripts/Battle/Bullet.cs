using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Guid bulletID;
    public float speed = 120f; // 移动速度
    public BaseTank tank; // 发射者
    private GameObject skin; // 炮弹模型
    private Rigidbody mRigidbody; // 物理
    private CancellationTokenSource cts;

    private void OnUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void Init(Guid guid,Dictionary<Guid, Bullet> BulletDic)
    {
        bulletID = guid;
        ResManager.Instance.LoadAssetAsync<GameObject>("BulletPrefab", false,
        onLoaded: async handle =>
        {
            cts = new CancellationTokenSource();
            skin = Instantiate(handle, this.transform);
            skin.transform.localPosition = Vector3.zero;
            skin.transform.localEulerAngles = Vector3.zero;

            // 物理
            mRigidbody = gameObject.AddComponent<Rigidbody>();
            mRigidbody.useGravity = false;

            try
            {
                // 等待5秒或直到取消
                await UniTask.Delay(5000, cancellationToken: cts.Token);

                // 再次检查状态
                if (!cts.Token.IsCancellationRequested)
                {
                    Debug.Log($"5秒超时销毁子弹");
                    BulletDic.Remove(bulletID);
                    Destroy(gameObject); // 使用安全销毁方法
                }
            }
            catch (OperationCanceledException)
            {
                this.Log($"5秒等待被取消");
            }
        },
        error =>
        {
            Debug.LogError($"Bullet.Init初始化执行异常");
            if (gameObject != null) Destroy(gameObject);
        }).Forget();
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        // 打到的坦克
        GameObject collObj = collisionInfo.gameObject;
        BaseTank hitTank = collObj.GetComponent<BaseTank>();

        if (hitTank == tank) // 不能打自己
            return;
        if (hitTank != null) // 攻击其他坦克
            SendMsgHit(tank, hitTank);

        Explosion();

        MsgFire msg = new MsgFire();
        msg.ID = GameMain.ID;
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = 0;
        msg.ey = 0;
        msg.ez = 0;
        msg.IsExplosion = true;
        NetManager.Send(msg);
    }

    /// <summary>
    /// 发送伤害协议。tank-发射者 hitTank-被攻击坦克
    /// </summary>
    private void SendMsgHit(BaseTank tank, BaseTank hitTank)
    {
        if (hitTank == null || tank == null)
            return;
        // 不是自己发出的炮弹
        if (tank.ID != GameMain.ID)
            return;
        MsgHit msg = new MsgHit();
        msg.targetID = hitTank.ID;
        msg.ID = tank.ID;
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        NetManager.Send(msg);
    }

    private void OnDestroy()
    {
        Debug.Log($"销毁物体的ID:{gameObject.GetInstanceID()}");

        GloablMono.Instance.OnUpdate -= OnUpdate;
        ResManager.Instance.ReleaseResource("BulletPrefab");
    }

    /// <summary>
    /// 显示爆炸效果
    /// </summary>
    public void Explosion()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
        ResManager.Instance.LoadAssetAsync<GameObject>("Fire", false,
        onLoaded: handle =>
        {
            Instantiate(handle, transform.position, transform.rotation);
            // 摧毁自身
            Destroy(gameObject);
        },
        error =>
        {
            Debug.LogError($"Bullet.OnCollisionEnter执行异常:{error}");
        }).Forget();
    }
}