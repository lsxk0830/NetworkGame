using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Guid bulletID;
    public long ID; // 发射者ID
    public float speed = 120f; // 移动速度
    private float lifeTime = 5f; // 子弹生命周期

    private void OnUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        // 检测生命周期
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            PoolReset();
        }
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        MsgFire msg = this.GetObjInstance<MsgFire>();
        msg.ID = ID; // 发射者ID
        msg.bulletID = bulletID; // 子弹ID
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = 0;
        msg.ey = 0;
        msg.ez = 0;
        msg.IsExplosion = true;
        NetManager.Send(msg);
        this.PushPool(msg); // 将消息对象归还对象池
        this.PushGameObject(this.gameObject); // 将子弹归还对象池
        this.GetGameObject(BattleManager.Instance.HitPrefab)
            .GetComponent<Hit>()
            .PoolInit(this.transform);

        // 打到的坦克
        GameObject collObj = collisionInfo.gameObject;
        if (collObj.TryGetComponent<BaseTank>(out BaseTank hitTank))
        {
            if (hitTank.ID == ID) return;// 不能打自己
            SendMsgHit(ID, hitTank.ID);
        }
    }

    /// <summary>
    /// 发送伤害协议。attackTankID-发射者 hitTankID-被攻击坦克
    /// </summary>
    private void SendMsgHit(long attackTankID, long hitTankID)
    {
        if (hitTankID == 0 || hitTankID == 0)
            return;

        if (attackTankID != GameMain.ID) return;// 不是自己发出的炮弹
        MsgHit msg = this.GetObjInstance<MsgHit>();
        msg.targetID = hitTankID;
        msg.ID = attackTankID;
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        NetManager.Send(msg);
        this.PushPool(msg);
    }

    /// <summary>
    /// 初始化子弹
    /// </summary>
    /// <param name="id">发射者ID</param>
    /// <param name="bulletGuid">子弹ID</param>
    /// <param name="position">初始位置</param>
    /// <param name="rotation">初始旋转</param>
    public void PoolInit(long id, Guid bulletGuid, Vector3 position, Quaternion rotation)
    {
        ID = id;
        bulletID = bulletGuid;
        transform.position = position;
        transform.rotation = rotation;
        lifeTime = 5f; // 重置生命周期
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    public void PoolReset()
    {
        this.PushGameObject(this.gameObject); // 将子弹归还对象池
        GloablMono.Instance.OnUpdate -= OnUpdate;
    }
}