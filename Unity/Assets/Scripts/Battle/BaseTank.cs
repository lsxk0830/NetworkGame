using System;
using UnityEngine;

public class BaseTank : MonoBehaviour
{
    public float steer = 20; // 转向速度
    public float turretSpeed = 30f; // 炮塔旋转速度
    public Transform turret; // 炮塔
    public Transform gun; // 炮管
    public Transform firePoint; // 发射点
    public float fired = 0.5f; // 炮弹Cd时间
    public float lastFireTime = 0; // 上一次发射炮弹时间
    public float hp = 100;
    public long ID; // 哪一玩家
    public int camp = 0; // 阵营
    protected Rigidbody mRigidbody;

    public virtual void Init(Player tankInfo)
    {
        camp = tankInfo.camp;
        ID = tankInfo.ID;
        hp = tankInfo.hp;

        transform.position = new Vector3(tankInfo.x, tankInfo.y, tankInfo.z);
        transform.eulerAngles = new Vector3(tankInfo.ex, tankInfo.ey, tankInfo.ez);

        mRigidbody = GetComponent<Rigidbody>();
        turret = transform.Find("Tank/Turret");
        gun = turret.transform.Find("Gun");
        firePoint = turret.transform.Find("FirePoint");
        Vector3 v3 = firePoint.transform.position;
        v3.y = 1;
        firePoint.transform.position = v3;
    }

    /// <summary>
    /// 开火
    /// </summary>
    public Bullet Fire(Guid bulletGuid)
    {
        if (isDie()) return null;
        Debug.Log($"开火");
        Bullet bullet = this.GetGameObject(EffectManager.BulletPrefab)
            .GetComponent<Bullet>();
        Vector3 targetPos = firePoint.transform.position + bullet.transform.forward * 50f;
        bullet.PoolInit(ID, bulletGuid, firePoint.position, targetPos);
        BulletManager.AddBullet(bullet);
        // 更新时间
        lastFireTime = Time.time;
        return bullet;
    }

    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool isDie()
    {
        return hp <= 0;
    }

    /// <summary>
    /// 被攻击
    /// </summary>
    public void Attacked(long winID, float att)
    {
        if (isDie()) return;

        hp -= att;
        if (isDie())
        {
            GameObject explosion = this.GetGameObject(EffectManager.DiePrefab);
            explosion.transform.position = transform.position;
            explosion.transform.rotation = transform.rotation;
            BaseTank winTank = BattleManager.GetTank(winID);
            MsgEndBattle msg = this.GetObjInstance<MsgEndBattle>();
            msg.winCamp = winTank.camp;
            NetManager.Instance.Send(msg);
            this.PushPool(msg);
        }
    }
}