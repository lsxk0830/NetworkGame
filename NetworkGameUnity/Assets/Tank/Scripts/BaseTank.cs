using UnityEngine;

public class BaseTank : MonoBehaviour
{
    /// <summary>
    /// 坦克模型资源
    /// </summary>
    private GameObject skin;

    protected Rigidbody mRigidbody;

    public virtual void Init(string skinPath)
    {
        // 皮肤
        GameObject skinRes = ResManager.LoadPrefab("tankPrefab");
        GameObject skin = Instantiate(skinRes);
        skin.transform.parent = this.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;

        // 物理
        mRigidbody = gameObject.AddComponent<Rigidbody>();
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, 2.5f, 1.47f);
        boxCollider.size = new Vector3(7, 5, 12);
    }

    public void Update()
    {

    }
}
