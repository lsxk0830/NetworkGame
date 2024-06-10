using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 100f; // 移动速度
    public BaseTank tank; // 发射者
    private GameObject skin; // 炮弹模型
    private Rigidbody mRigidbody; // 物理

    public void Init()
    {
        // 皮肤
        GameObject skinRes = ResManager.LoadPrefab("bulletPrefab");
        skin = Instantiate(skinRes);
        skin.transform.parent = this.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;

        // 物理
        mRigidbody = gameObject.AddComponent<Rigidbody>();
        mRigidbody.useGravity = false;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        // 打到的坦克
        GameObject collObj = collisionInfo.gameObject;
        BaseTank hitTank = collObj.GetComponent<BaseTank>();
        if (hitTank == tank) return; // 不能打自己
        // 显示爆炸效果
        GameObject explode = ResManager.LoadPrefab("fire");
        Instantiate(explode, transform.position, transform.rotation);
        // 摧毁自身
        Destroy(gameObject);
    }
}
