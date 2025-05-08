using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameMain : MonoBehaviour
{
    public static string id = ""; // 玩家角色ID

    private void Awake()
    {
        new GameObject("MonoUpdate").AddComponent<GloablMono>();

        PanelManager.Init();
        PanelManager.Open<LoginPanel>();
        GloablMono.Instance.OnUpdate += OnUpdate;

        EventSystem.RegisterEvent(Events.SocketOnConnectFail, OnConnectClose);
        // 网络消息监听
        EventSystem.RegisterEvent(Events.MsgKick, OnMsgKick);
        NetManager.ConnectAsync(); // 循环连接连接服务器
    }


    private void OnUpdate()
    {
        NetManager.Update();
    }

    private void OnConnectClose(string err)
    {
        Debug.LogError("断开连接");
        PanelManager.Open<TipPanel>(err);
        NetManager.ConnectAsync(); // 循环连接连接服务器
    }

    private void OnMsgKick(MsgBase msgBse)
    {
        PanelManager.Open<TipPanel>("被踢下线");
    }

    private void OnDestroy()
    {
        GloablMono.Instance.OnUpdate -= OnUpdate;
        EventSystem.RemoveEvent(Events.SocketOnConnectFail, OnConnectClose);
    }
}