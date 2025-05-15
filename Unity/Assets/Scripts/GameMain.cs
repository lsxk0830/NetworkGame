using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static long ID; // 玩家角色ID
    public static bool NetConnect = false;

    private void Awake()
    {
        new GameObject("MonoUpdate").AddComponent<GloablMono>();
        GloablMono.Instance.OnUpdate += OnUpdate;

        EventManager.Instance.RegisterEvent(Events.SocketOnConnectSuccess, OnConnectSuccess);
        EventManager.Instance.RegisterEvent(Events.SocketOnConnectFail, OnConnectFail);
        EventManager.Instance.RegisterEvent(Events.PanelLoadSuccess, OnPanelLoadSuccess);
        EventManager.Instance.RegisterEvent(Events.MsgKick, OnMsgKick);
        EventManager.Instance.RegisterEvent(Events.MsgPing, OnPong);
        PanelManager.Instance.Init();
        NetManager.ConnectAsync(); // 循环连接服务器
    }

    private void OnUpdate()
    {
        NetManager.Update();
    }

    private void OnConnectSuccess(string msg)
    {
        NetConnect = true;
        Debug.Log("服务器连接成功");
    }

    private void OnConnectFail(string err)
    {
        Debug.LogError("断开连接");
        PanelManager.Instance.Open<TipPanel>(err);
        NetManager.ConnectAsync(); // 循环连接连接服务器
    }

    private void OnMsgKick(MsgBase msgBse)
    {
        PanelManager.Instance.Open<TipPanel>("被踢下线");
    }

    private void OnPanelLoadSuccess()
    {
        Debug.Log("打开登录界面");
        PanelManager.Instance.Open<LoginPanelView>();
        EventManager.Instance.RemoveEvent(Events.PanelLoadSuccess, OnPanelLoadSuccess);
    }

    private void OnPong(MsgBase msgBse)
    {
        Debug.Log(msgBse.protoName);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEvent(Events.SocketOnConnectSuccess, OnConnectSuccess);
        EventManager.Instance.RemoveEvent(Events.SocketOnConnectFail, OnConnectFail);
        EventManager.Instance.RemoveEvent(Events.MsgKick, OnMsgKick);
    }
}