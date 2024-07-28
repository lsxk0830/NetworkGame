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
        layers.Add(Layer.Tip, panel);
    }

    /// <summary>
    /// 打开面板
    /// </summary>

    public static void Open<T>(params object[] para) where T : BasePanel
    {

    }

    /// <summary>
    /// 关闭面板
    /// </summary>

    public static void Close(string name)
    {

    }
}