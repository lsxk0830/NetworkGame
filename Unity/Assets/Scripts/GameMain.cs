using UnityEngine;

public class GameMain : MonoSingleton<GameMain>
{
    public static long ID; // 用户ID
    public static bool NetConnect = false;
    public static GameObject tankModel; // 坦克模型

    protected override void OnAwake()
    {
        GameObject MonoTool = new GameObject("MonoTool");
        MonoTool.AddComponent<GloablMono>();
        transform.Find("BGMusicManager").gameObject.AddComponent<BGMusicManager>();

        EventManager.Instance.RegisterEvent(Events.SocketOnConnectSuccess, OnConnectSuccess);
        EventManager.Instance.RegisterEvent(Events.SocketOnConnectFail, OnConnectFail);
        EventManager.Instance.RegisterEvent(Events.PanelLoadSuccess, OnPanelLoadSuccess);
        EventManager.Instance.RegisterEvent(Events.MsgKick, OnMsgKick);
        EventManager.Instance.RegisterEvent(Events.MsgPing, OnPong);
        PanelManager.Instance.Init();
        NetManager.Instance.ConnectAsync(); // 循环连接服务器
        ResManager.Instance.LoadAssetsAsync<GameObject>("TankModel", handle =>
        {
            tankModel = Instantiate(handle);
            tankModel.name = "TankModel";
            DontDestroyOnLoad(tankModel);
        }).Forget();

        Init();
    }

    private void Init()
    {
        bool activeMusic = PlayerPrefs.GetInt("Toggle_Music") == 1 ? true : false;
        float m = PlayerPrefs.GetFloat("Slider_Music");
        BGMusicManager.Instance.ChangeOpen(activeMusic);
        BGMusicManager.Instance.ChangeValue(m);
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
        PanelManager.Instance.Open<TipPanel>("被踢下线", (System.Action)OpenLoginPanel);
    }

    private void OpenLoginPanel()
    {
        PanelManager.Instance.CloseAllExceptOther<LoginPanelView>();
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

    private void OnApplicationQuit()
    {
        EventManager.Instance.RemoveEvent(Events.SocketOnConnectSuccess, OnConnectSuccess);
        EventManager.Instance.RemoveEvent(Events.SocketOnConnectFail, OnConnectFail);
        EventManager.Instance.RemoveEvent(Events.MsgKick, OnMsgKick);
        NetManager.Instance.Close();
        Debug.LogError("应用程序退出，断开连接");
    }
}