using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public string skinPath; // 皮肤路径
    public GameObject skin; // 皮肤
    public PanelManager.Layer layer = PanelManager.Layer.Panel; // 层级

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        GameObject skinPrefab = ResManager.LoadPrefab(skinPath);
        skin = Instantiate(skinPrefab);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        string name = this.GetType().ToString();
        PanelManager.Close(name);
    }

    public virtual void OnInit() { } // 初始化时

    public virtual void OnShow(params object[] para) { } // 显示时

    public virtual void OnClose() { } // 关闭时
}
