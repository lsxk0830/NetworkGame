using System;
using System.Collections.Generic;
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
    protected Dictionary<Guid, Bullet> BulletDic;

    private Vector3 pos = new Vector3();
    private Vector3 rot = new Vector3();

    public virtual void Init(Player tankInfo)
    {
        BulletDic = new Dictionary<Guid, Bullet>();
        camp = tankInfo.camp;
        ID = tankInfo.ID;
        hp = tankInfo.hp;
        pos.x = tankInfo.x; pos.y = tankInfo.y; pos.z = tankInfo.z;
        rot.x = tankInfo.ex; rot.y = tankInfo.ey; rot.z = tankInfo.ez;
        transform.position = pos;
        transform.eulerAngles = rot;

        mRigidbody = GetComponent<Rigidbody>();
        turret = transform.Find("Tank/Turret");
        gun = turret.transform.Find("Gun");
        firePoint = turret.transform.Find("FirePoint");
    }

    /// <summary>
    /// 开火
    /// </summary>
    public Bullet Fire(Guid bulletGuid)
    {
        if (isDie()) return null;
        Debug.Log($"开火");
        Bullet bullet = this.GetGameObject(BattleManager.Instance.BulletPrefab).GetComponent<Bullet>();
        BulletDic.Add(bulletGuid, bullet);
        bullet.PoolInit(ID, bulletGuid, firePoint.position, firePoint.rotation);
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
            GameObject explosion = this.GetGameObject(BattleManager.Instance.DiePrefab);
            explosion.transform.position = transform.position;
            explosion.transform.rotation = transform.rotation;
            BaseTank winTank = BattleManager.GetTank(winID);
            MsgEndBattle msg = new MsgEndBattle()
            {
                winCamp = winTank.camp
            };
            NetManager.Send(msg);
        }
    }
}