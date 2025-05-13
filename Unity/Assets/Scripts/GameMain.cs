using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static string id = ""; // 玩家角色ID

    private void Awake()
    {
        new GameObject("MonoUpdate").AddComponent<GloablMono>();
        GloablMono.Instance.OnUpdate += OnUpdate;

        EventSystem.RegisterEvent(Events.SocketOnConnectSuccess, OnConnectSuccess);
        EventSystem.RegisterEvent(Events.SocketOnConnectFail, OnConnectFail);
        EventSystem.RegisterEvent(Events.MsgKick, OnMsgKick);
        EventSystem.RegisterEvent(Events.PanelLoadSuccess, OnPanelLoadSuccess);

        PanelManager.Init();
        NetManager.ConnectAsync(); // 循环连接服务器
    }

    private void OnUpdate()
    {
        NetManager.Update();
    }

    private void OnConnectSuccess(string msg)
    {
        Debug.Log("服务器连接成功");
    }

    private void OnConnectFail(string err)
    {
        Debug.LogError("断开连接");
        PanelManager.Open<TipPanel>(err);
        NetManager.ConnectAsync(); // 循环连接连接服务器
    }

    private void OnMsgKick(MsgBase msgBse)
    {
        PanelManager.Open<TipPanel>("被踢下线");
    }

    private void OnPanelLoadSuccess()
    {
        Debug.Log("打开登录界面");
        PanelManager.Open<LoginPanelView>();
        EventSystem.RemoveEvent(Events.PanelLoadSuccess, OnPanelLoadSuccess);
    }

    private void OnDestroy()
    {
        EventSystem.RemoveEvent(Events.SocketOnConnectSuccess, OnConnectSuccess);
        EventSystem.RemoveEvent(Events.SocketOnConnectFail, OnConnectFail);
        EventSystem.RemoveEvent(Events.MsgKick, OnMsgKick);
    }
}