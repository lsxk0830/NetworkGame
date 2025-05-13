using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public string panelName; // 皮肤路径
    public PanelManager.Layer layer = PanelManager.Layer.Panel; // 层级

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        string name = this.GetType().FullName;
        //Debug.Log($"关闭：{name}");
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
