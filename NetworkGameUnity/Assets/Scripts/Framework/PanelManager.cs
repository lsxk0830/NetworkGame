using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class PanelManager
{
    public enum Layer { Panel, Tip } // 层级

    private static Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>(); // 层级列表
    public static Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>(); // 面板列表
    private static Dictionary<string, GameObject> panelCache = new Dictionary<string, GameObject>(); //aaddressable缓存

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        //Transform root = GameObject.Find("Root").transform;
        Transform canvas = GameObject.Find("Canvas").transform;
        Transform panel = canvas.Find("Panel");
        Transform tip = canvas.Find("Tip");
        layers.Add(Layer.Panel, panel);
        layers.Add(Layer.Tip, tip);

        Addressables.LoadAssetsAsync<GameObject>("UIPanel", panel =>
        {
            panelCache.Add(panel.name, panel);
            Debug.Log($"加载面板：{panel.name}");
            if (panel.name == typeof(LoginPanel).FullName)
                EventSystem.InvokeEvent(Events.PanelLoadSuccess); // 触发面板加载成功事件
        }).Completed += operation => { Addressables.Release(operation); };
    }

    /// <summary>
    /// 打开面板
    /// </summary>
    public static void Open<T>(params object[] para) where T : BasePanel
    {
        string panelName = typeof(T).ToString();
        if (panels.ContainsKey(panelName))
        {
            panels[panelName].OnShow();
            return;
        }

        if (!panelCache.ContainsKey(panelName)) return;

        GameObject go = GameObject.Instantiate(panelCache[panelName]);
        go.name = panelName;
        BasePanel panel = go.AddComponent<T>();
        go.transform.SetParent(layers[panel.layer],false);
        panels.Add(panelName, panel);
        panel.OnInit();
        panel.OnShow(para);

        panelCache.Remove(panelName); // 从缓存中移除
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    public static void Close(string name)
    {
        if (!panels.ContainsKey(name)) // 没有打开
            return;
        BasePanel panel = panels[name];
        panel.OnClose();
    }
}