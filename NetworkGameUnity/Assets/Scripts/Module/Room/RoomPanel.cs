using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    private Button startBtn; // 开战按钮
    private Button closeBtn; // 退出按钮
    private Transform content; // 列表容器
    private GameObject playerObj; // 玩家信息物体

    public override void OnInit()
    {
        skinPath = "RoomPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        // 寻找组件
        startBtn = go.transform.Find("CtrlPanel/StartBtn").GetComponent<Button>();
        closeBtn = go.transform.Find("CtrlPanel/CloseBtn").GetComponent<Button>();
        content = go.transform.Find("ListPanel/ScrollView/Viewport/Content");
        playerObj = go.transform.Find("Player").gameObject;
        //不激活玩家信息
        playerObj.SetActive(false);
        //按钮事件
        startBtn.onClick.AddListener(OnStartClick);
        closeBtn.onClick.AddListener(OnCloseClick);
        // 协议监听

        EventSystem.RegisterEvent(Events.MsgGetRoomInfo, OnMsgGetRoomInfo);
        EventSystem.RegisterEvent(Events.MsgLeaveRoom, OnMsgLeaveRoom);
        EventSystem.RegisterEvent(Events.MsgStartBattle, OnMsgStartBattle);
        // 发送查询
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        NetManager.Send(msg);
    }

    public override void OnClose()
    {
        // 协议取消监听
        EventSystem.RemoveEvent(Events.MsgGetRoomInfo, OnMsgGetRoomInfo);
        EventSystem.RemoveEvent(Events.MsgLeaveRoom, OnMsgLeaveRoom);
        EventSystem.RemoveEvent(Events.MsgStartBattle, OnMsgStartBattle);
    }

    #region 协议事件

    /// <summary>
    /// 收到玩家列表协议
    /// </summary>
    private void OnMsgGetRoomInfo(MsgBase msgBse)
    {
        MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBse;
        Debug.Log($"收到玩家列表协议");
        // 清除玩家列表
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            GameObject go = content.GetChild(i).gameObject;
            Destroy(go);
        }
        // 重新生成列表
        if (msg.Players == null) return;

        for (int i = 0; i < msg.Players.Length; i++)
        {
            GeneratePlayerInfo(msg.Players[i]);
        }
    }

    /// <summary>
    /// 收到退出房间协议
    /// </summary>
    private void OnMsgLeaveRoom(MsgBase msgBse)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBse;
        Debug.Log($"收到退出房间协议");
        if (msg.result == 0) // 成功退出房间
        {
            PanelManager.Open<TipPanel>("退出房间");
            PanelManager.Open<HomePanel>();
            Close();
        }
        else
            PanelManager.Open<TipPanel>("退出房间失败");
    }

    /// <summary>
    /// 收到开战协议
    /// </summary>
    private void OnMsgStartBattle(MsgBase msgBse)
    {
        Debug.Log($"收到开战协议");
        MsgStartBattle msg = (MsgStartBattle)msgBse;
        if (msg.result == 0)//开战
            Close();
        else // 开战失败
            PanelManager.Open<TipPanel>("开战失败!两队至少都需要一名玩家，只有队长可以开始战斗！");
    }

    /// <summary>
    /// 创建一个玩家信息单元
    /// </summary>
    private void GeneratePlayerInfo(PlayerInfo playerInfo)
    {
        GameObject go = Instantiate(playerObj);
        go.transform.SetParent(content);
        go.SetActive(true);
        go.transform.localScale = Vector3.one;
        // 获取组件
        Transform trans = go.transform;
        TMP_Text idText = trans.Find("IdText").GetComponent<TMP_Text>();
        TMP_Text campText = trans.Find("CampText").GetComponent<TMP_Text>();
        TMP_Text scoreText = trans.Find("ScoreText").GetComponent<TMP_Text>();
        // 填充信息
        idText.text = playerInfo.id;
        campText.text = playerInfo.camp == 1 ? "红" : "蓝";
        if (playerInfo.isOwner == 1)
            campText.text = campText.text + "!";
        scoreText.text = playerInfo.win + "胜" + playerInfo.lost + "负";
    }


    #endregion

    #region UI事件

    /// <summary>
    /// 点击开战按钮
    /// </summary>
    private void OnStartClick()
    {
        Debug.Log($"发送开始战斗协议");
        MsgStartBattle msg = new MsgStartBattle();
        NetManager.Send(msg);
    }

    /// <summary>
    /// 点击退出按钮
    /// </summary>
    private void OnCloseClick()
    {
        Debug.Log($"发送退出房间协议");
        MsgLeaveRoom msg = new MsgLeaveRoom();
        NetManager.Send(msg);
    }

    #endregion
}