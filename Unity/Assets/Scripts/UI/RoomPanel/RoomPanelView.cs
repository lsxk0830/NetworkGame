using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanelView : BasePanel
{
    private Button startBtn; // 开战按钮
    private Button closeBtn; // 退出按钮
    private Transform content; // 列表容器
    private GameObject playerObj; // 玩家信息物体
    private RoomPanelController controller;

    public override void OnInit()
    {
        layer = PanelManager.Layer.Panel;
        // 寻找组件
        startBtn = transform.Find("CtrlPanel/StartBtn").GetComponent<Button>();
        closeBtn = transform.Find("CtrlPanel/CloseBtn").GetComponent<Button>();
        content = transform.Find("ListPanel/ScrollView/Viewport/Content");
        playerObj = transform.Find("Player").gameObject;
        controller = new RoomPanelController(this);
    }

    public override void OnShow(params object[] para)
    {
        gameObject.SetActive(true);

        DeleteLastGo();
        if (para[0].GetType() == typeof(MsgCreateRoom)) //创建房间
            UpdateUICreateRoom((MsgCreateRoom)para[0]);
        else if (para[0].GetType() == typeof(MsgEnterRoom)) // 进入房间
            UpdateUIEnterRoom((MsgEnterRoom)para[0]);
        else
            PanelManager.Instance.Open<TipPanel>("打开房间异常");

        //按钮事件
        startBtn.onClick.AddListener(OnStartClick);
        closeBtn.onClick.AddListener(OnCloseClick);
        controller.AddListener();
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
        controller.RemoveListener();
    }

    #region 创建一个玩家信息单元

    /// <summary>
    /// 创建一个玩家信息单元
    /// </summary>
    public void GeneratePlayerInfo(Player playerInfo)
    {
        GameObject go = Instantiate(playerObj, content);
        go.name = playerInfo.ID.ToString();
        go.SetActive(true);
        go.transform.localScale = Vector3.one;
        // 获取组件
        Transform trans = go.transform;
        TMP_Text idText = trans.Find("IdText").GetComponent<TMP_Text>();
        TMP_Text campText = trans.Find("CampText").GetComponent<TMP_Text>();
        TMP_Text scoreText = trans.Find("ScoreText").GetComponent<TMP_Text>();
        // 填充信息
        idText.text = playerInfo.ID.ToString();
        campText.text = playerInfo.camp == 1 ? "红" : "蓝";
        if (playerInfo.isOwner == 1)
            campText.text = campText.text + "!";
        //scoreText.text = playerInfo.win + "胜" + playerInfo.lost + "负";
    }

    #endregion

    #region UI事件

    /// <summary>
    /// 点击开战按钮
    /// </summary>
    private void OnStartClick()
    {
        controller.OnStartClick();
    }

    /// <summary>
    /// 点击退出按钮
    /// </summary>
    private void OnCloseClick()
    {
        controller.OnCloseClick();
    }

    #endregion

    #region  更新UI

    /// <summary>
    /// 创建房间时更新UI
    /// </summary>
    private void UpdateUICreateRoom(MsgCreateRoom response)
    {
        Player player = new Player()
        {
            ID = GameMain.ID,
            roomId = response.roomID
        };
        Debug.Log($"创建房间");
        controller.roomID = response.roomID;
        GeneratePlayerInfo(player);
    }

    /// <summary>
    /// 进入房间时更新UI
    /// </summary>
    public void UpdateUIEnterRoom(MsgEnterRoom response)
    {
        for (int i = content.childCount - 1; i >= 0; i--) // 删除之前的
        {
            GameObject go = content.GetChild(i).gameObject;
            Destroy(go);
        }
        if (response.players == null) return;
        for (int i = 0; i < response.players.Length; i++)// 重新生成
        {
            GeneratePlayerInfo(response.players[i]);
        }
        controller.roomID = response.roomID;
        Debug.Log($"进入房间");
    }

    /// <summary>
    /// 根据roomID删除指定的物体
    /// </summary>
    public void DeleteGoByRoomID(string roomId)
    {
        for (int i = content.childCount - 1; i >= 0; i--) // 删除指定玩家
        {
            GameObject go = content.GetChild(i).gameObject;
            if (go.name == roomId)
            {
                Destroy(go);
                break;
            }
        }
    }

    /// <summary>
    /// 删除之前的物体
    /// </summary>
    public void DeleteLastGo()
    {
        for (int i = content.childCount - 1; i >= 0; i--) // 删除指定玩家
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    #endregion 跟新UI
}