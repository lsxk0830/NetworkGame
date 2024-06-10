using Unity.VisualScripting;
using UnityEngine;

public class BaseTank : MonoBehaviour
{
    /// <summary>
    /// 坦克模型资源
    /// </summary>
    private GameObject skin;

    public virtual void Init(string skinPath)
    {
        GameObject skinRes = ResManager.LoadPrefab("tankPrefab");
        GameObject skin = Instantiate(skinRes);
        skin.transform.parent = this.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;
    }

    public void Update()
    {

    }
}
