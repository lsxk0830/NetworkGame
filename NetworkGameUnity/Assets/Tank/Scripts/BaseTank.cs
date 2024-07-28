using System;
using UnityEngine;

public class BaseTank : MonoBehaviour
{
    /// <summary>
    /// 坦克模型资源
    /// </summary>
    private GameObject skin;

    public float steer = 20; // 旋转角度
    public float speed = 6; // 移动速度

    public float turretSpeed = 30f; // 炮塔旋转速度
    public Transform turret; // 炮塔
    public Transform gun; // 炮管
    public Transform firePoint; // 发射点

    public float fired = 0.5f; // 炮弹Cd时间
    public float lastFireTime = 0; // 上一次发射炮弹时间

    public float hp = 100;

    protected Rigidbody mRigidbody;

    public virtual void Init(string skinPath)
    {
        // 皮肤
        GameObject skinRes = ResManager.LoadPrefab(skinPath);
        GameObject skin = Instantiate(skinRes);
        skin.transform.parent = this.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;

        // 物理
        mRigidbody = gameObject.AddComponent<Rigidbody>();
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, 2.5f, 1.47f);
        boxCollider.size = new Vector3(7, 5, 12);

        // 炮塔炮管
        turret = skin.transform.Find("Turret");
        gun = turret.transform.Find("Gun");
        firePoint = gun.transform.Find("FirePoint");
    }

    /// <summary>
    /// 开火
    /// </summary>
    public Bullet Fire()
    {
        if (isDie()) return null;

        // 产生炮弹
        GameObject bulletObj = new GameObject("bullet");
        Bullet bullet = bulletObj.AddComponent<Bullet>();
        bullet.Init();
        bullet.tank = this;

        // 位置
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        // 更新时间
        lastFireTime = Time.time;
        return bullet;
    }

    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool isDie()
    {
        return hp < 0;
    }

    /// <summary>
    /// 被攻击
    /// </summary>
    public void Attacked(float att)
    {
        if (isDie())
            return;

        hp -= att;
        if (isDie())
        {
            GameObject obj = ResManager.LoadPrefab("Explosion");
            GameObject explode = Instantiate(obj, transform.position, transform.rotation);
            explode.transform.SetParent(transform);
        }
    }
}
