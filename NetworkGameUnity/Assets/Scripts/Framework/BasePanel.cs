using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BasePanel : MonoBehaviour
{
    public string skinPath; // 皮肤路径
    public GameObject go;
    public PanelManager.Layer layer = PanelManager.Layer.Panel; // 层级

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        Addressables.LoadAssetAsync<GameObject>(skinPath).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject skinPrefab = handle.Result;
                go = Instantiate(skinPrefab);

                Addressables.Release(handle);
            }
        };
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        string name = this.GetType().ToString();
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
