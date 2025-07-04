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
    }

    /// <summary>
    /// 开火
    /// </summary>
    public void SyncFire(MsgFire msg)
    {
        Vector3 pos = new Vector3(msg.x, 1, msg.z);
        Vector3 tPos = new Vector3(msg.tx, 1, msg.tz);
        if (msg.IsExplosion)
        {
            this.GetGameObject(EffectManager.HitPrefab)
                .GetComponent<Hit>()
                .PoolInit(tPos);
            BulletManager.GetBullet(msg.bulletID)?.PoolReset(); // 将子弹归还对象池
            BulletManager.RemoveBullet(msg.bulletID); // 从字典中移除子弹
        }
        else
        {
            Bullet bullet = this.GetGameObject(EffectManager.BulletPrefab).GetComponent<Bullet>();
            bullet.PoolInit(ID, msg.bulletID, pos, tPos);
            BulletManager.AddBullet(bullet);
            lastFireTime = Time.time;
        }
    }

    /// <summary>
    /// 被攻击
    /// </summary>
    /// <param name="attackID">攻击者ID</param>
    /// <param name="hp">剩余血量</param>
    /// <param name="att">攻击力</param>
    public void Attacked(long attackID,int hp, float att)
    {
        if (hp <= 0) return;
        Debug.LogError($"坦克{ID}被{attackID}攻击，剩余血量：{hp},攻击力：{att}");

        if (hp <= 0)
        {
            GameObject explosion = this.GetGameObject(EffectManager.DiePrefab);
            explosion.transform.position = transform.position;
            explosion.transform.rotation = transform.rotation;
            mRigidbody.constraints = RigidbodyConstraints.FreezeAll; // 冻结刚体
        }
    }
}