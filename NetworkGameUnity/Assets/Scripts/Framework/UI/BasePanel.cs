using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public string skinPath; // 皮肤路径
    public GameObject skin; // 皮肤【面板或者弹窗的那个物体】
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

    /// <summary>
    /// 初始化时
    /// </summary>
    public virtual void OnInit() { }

    /// <summary>
    /// 显示
    /// </summary>
    public virtual void OnShow(params object[] args) { }

    /// <summary>
    /// 关闭
    /// </summary>
    public virtual void OnClose() { }
}
