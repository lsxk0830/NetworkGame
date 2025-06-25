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
        Vector3 pos = new Vector3(tankInfo.x, tankInfo.y, tankInfo.z);
        Vector3 rot = new Vector3(tankInfo.ex, tankInfo.ey, tankInfo.ez);
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
            ResManager.Instance.LoadAssetAsync<GameObject>("Explosion", false,
            handle =>
            {
                GameObject explosion = Instantiate(handle.gameObject, transform.position, transform.rotation);
                explosion.transform.SetParent(transform);
                BaseTank winTank = BattleManager.GetTank(winID);
                MsgEndBattle msg = new MsgEndBattle()
                {
                    winCamp = winTank.camp
                };
                NetManager.Send(msg);
            },
            error =>
            {
                Debug.LogError($"BaseTank.Attacked被攻击执行异常");
            }).Forget();

        }
    }
}