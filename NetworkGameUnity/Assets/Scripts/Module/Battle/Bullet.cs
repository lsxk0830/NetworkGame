using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 120f; // 移动速度
    public BaseTank tank; // 发射者
    private GameObject skin; // 炮弹模型
    private Rigidbody mRigidbody; // 物理

    private void Start()
    {
        GloablMono.Instance.OnUpdate += f =>
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        };
    }

    public void Init()
    {
        // 皮肤
        GameObject skinRes = ResManager.LoadPrefab("BulletPrefab");
        skin = Instantiate(skinRes);
        skin.transform.parent = this.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;

        // 物理
        mRigidbody = gameObject.AddComponent<Rigidbody>();
        mRigidbody.useGravity = false;
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
        GameObject explode = ResManager.LoadPrefab("fire");
        Instantiate(explode, transform.position, transform.rotation);
        // 摧毁自身
        Destroy(gameObject);
    }

    /// <summary>
    /// 发送伤害协议。tank-发射者 hitTank-被攻击坦克
    /// </summary>
    private void SendMsgHit(BaseTank tank, BaseTank hitTank)
    {
        if (hitTank == null || tank == null)
            return;
        // 不是自己发出的炮弹
        if (tank.id != GameMain.id)
            return;
        MsgHit msg = new MsgHit();
        msg.targetId = hitTank.id;
        msg.id = tank.id;
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        NetManager.Send(msg);
    }
}