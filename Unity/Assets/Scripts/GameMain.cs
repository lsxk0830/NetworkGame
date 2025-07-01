using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static long ID; // 用户ID
    public static bool NetConnect = false;

    private void Awake()
    {
        GameObject MonoTool = new GameObject("MonoTool");
        MonoTool.AddComponent<GloablMono>();
        MonoTool.AddComponent<ResManager>();

        EventManager.Instance.RegisterEvent(Events.SocketOnConnectSuccess, OnConnectSuccess);
        EventManager.Instance.RegisterEvent(Events.SocketOnConnectFail, OnConnectFail);
        EventManager.Instance.RegisterEvent(Events.PanelLoadSuccess, OnPanelLoadSuccess);
        EventManager.Instance.RegisterEvent(Events.MsgKick, OnMsgKick);
        EventManager.Instance.RegisterEvent(Events.MsgPing, OnPong);
        PanelManager.Instance.Init();
        NetManager.Instance.ConnectAsync(); // 循环连接服务器

        DontDestroyOnLoad(gameObject);
    }

    private void OnUpdate()
    {
        NetManager.Instance.Update();
    }

    private void OnConnectSuccess(string msg)
    {
        GloablMono.Instance.OnUpdate += OnUpdate;
        NetConnect = true;
        Debug.Log("服务器连接成功");
    }

    private void OnConnectFail(string err)
    {
        GloablMono.Instance.OnUpdate -= OnUpdate;
        Debug.LogError("断开连接");
        GloablMono.Instance.TriggerFromOtherThread(() =>
        {
            PanelManager.Instance.Open<TipPanel>(err);
            NetManager.Instance.ConnectAsync(); // 在主线程中循环连接连接服务器
        });
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
        NetManager.Instance.Close();
    }
}