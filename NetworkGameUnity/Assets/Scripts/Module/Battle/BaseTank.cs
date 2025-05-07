using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BaseTank : MonoBehaviour
{
    private GameObject skin; // 坦克模型资源
    public float steer = 20; // 转向速度
    public float speed = 6; // 移动速度
    public float turretSpeed = 30f; // 炮塔旋转速度
    public Transform turret; // 炮塔
    public Transform gun; // 炮管
    public Transform firePoint; // 发射点
    public float fired = 0.5f; // 炮弹Cd时间
    public float lastFireTime = 0; // 上一次发射炮弹时间
    public float hp = 100;
    public string id = ""; // 哪一玩家
    public int camp = 0; // 阵营
    protected Rigidbody mRigidbody;

    public virtual AsyncOperationHandle Init(string tankName)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(tankName);
        handle.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject skinRes = handle.Result;
                skin = Instantiate(skinRes);
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
                //ToDo: 这里需要根据模型的实际结构来修改炮塔和炮管的名称
                //gun = turret.transform.Find("Gun");
                //firePoint = gun.transform.Find("FirePoint");
            }
        };
        return handle;

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
    public void Attacked(string winID, float att)
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
                MsgBattleResult msg = new MsgBattleResult()
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