using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static string id = ""; // 玩家角色ID

    void Start()
    {
        // 网络监听
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        // 初始化
        PanelManager.Init();
        BattleManager.Init();
        // 打开登录面板
        PanelManager.Open<LoginPanel>();
    }

    void Update()
    {
        NetManager.Update();
    }

    private void OnConnectClose(string err)
    {
        Debug.Log("断开连接");
    }

    private void OnMsgKick(MsgBase msgBse)
    {
        PanelManager.Open<TipPanel>("被踢下线");
    }
}