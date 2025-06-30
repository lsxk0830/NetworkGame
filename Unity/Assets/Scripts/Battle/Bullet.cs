using System;
using UnityEngine;

public class Bullet : MonoBehaviour, IPool
{
    public Guid bulletID;
    public long ID; // 发射者ID
    public float speed = 120f; // 移动速度

    private void OnUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        // 打到的坦克
        GameObject collObj = collisionInfo.gameObject;
        BaseTank hitTank = collObj.GetComponent<BaseTank>();

        if (hitTank.ID == ID) // 不能打自己
            return;
        if (hitTank != null) // 攻击其他坦克
            SendMsgHit(ID, hitTank.ID);

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
    /// 发送伤害协议。attackTankID-发射者 hitTankID-被攻击坦克
    /// </summary>
    private void SendMsgHit(long attackTankID, long hitTankID)
    {
        if (hitTankID == 0 || hitTankID == 0)
            return;

        if (attackTankID != GameMain.ID) return;// 不是自己发出的炮弹
        MsgHit msg = new MsgHit();
        msg.targetID = hitTankID;
        msg.ID = attackTankID;
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        NetManager.Send(msg);
    }

    /// <summary>
    /// 显示爆炸效果
    /// </summary>
    public void Explosion()
    {
        this.GetGameObject(BattleManager.Instance.Fire);
        this.PushGameObject(this.gameObject);
    }

    public void PoolInit()
    {
        BulletInit();
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    public void BulletInit(Vector3 position, Quaternion rotation)
    {
        // 初始化位置和旋转
        transform.position = position;
        transform.rotation = rotation;
    }

    public void PoolReset()
    {
        GloablMono.Instance.OnUpdate -= OnUpdate;
    }
}