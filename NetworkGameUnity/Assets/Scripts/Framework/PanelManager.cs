using System.Collections.Generic;
using UnityEngine;

public static class PanelManager
{
    public enum Layer
    {
        Panel,
        Tip
    }

    private static Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>(); // 层级列表

    public static Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>(); // 面板列表

    // 结构
    public static Transform root;
    public static Transform canvas;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        root = GameObject.Find("Root").transform;
        canvas = GameObject.Find("Canvas").transform;
        Transform panel = canvas.Find("Panel");
        Transform tip = canvas.Find("Tip");
        layers.Add(Layer.Panel, panel);
        layers.Add(Layer.Tip, tip);
    }

    /// <summary>
    /// 打开面板
    /// </summary>

    public static void Open<T>(params object[] para) where T : BasePanel
    {
        string name = typeof(T).ToString();
        if (panels.ContainsKey(name))
            return;

        // 组件
        BasePanel panel = root.gameObject.AddComponent<T>();
        panel.OnInit();
        panel.Init();
        // 父容器
        Transform layer = layers[panel.layer];
        panel.skin.transform.SetParent(layer, false);
        panels.Add(name, panel);
        panel.OnShow(para);
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
        panels.Remove(name);
        GameObject.Destroy(panel.skin); // 销毁面板
        Component.Destroy(panel); // 销毁脚本
    }
}