using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 120f; // 移动速度
    public BaseTank tank; // 发射者
    private GameObject skin; // 炮弹模型
    private Rigidbody mRigidbody; // 物理

    private void Start()
    {
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    private void OnUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void Init()
    {
        ResManager.Instance.LoadAssetAsync<GameObject>("BulletPrefab", false,
            handle =>
            {
                skin = Instantiate(handle, this.transform);
                skin.transform.localPosition = Vector3.zero;
                skin.transform.localEulerAngles = Vector3.zero;

                // 物理
                mRigidbody = gameObject.AddComponent<Rigidbody>();
                mRigidbody.useGravity = false;
            },
            error =>
            {
                Debug.LogError($"Bullet.Init初始化执行异常");
            }
        ).Forget();
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

        // 显示爆炸效果
        ResManager.Instance.LoadAssetAsync<GameObject>("Fire", false,
            handle =>
            {
                Instantiate(handle, transform.position, transform.rotation);
                // 摧毁自身
                Destroy(gameObject);
            },
            error =>
            {
                Debug.LogError($"Bullet.OnCollisionEnter执行异常");
            }
        ).Forget();
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
        GloablMono.Instance.OnUpdate -= OnUpdate;
    }
}