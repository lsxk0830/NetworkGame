using UnityEngine;

public class RoomPanelController
{
    private RoomPanelView view;
    public string roomID; // 房间ID

    public RoomPanelController(RoomPanelView view)
    {
        this.view = view;
    }

    public void AddListener()
    {
        EventManager.Instance.RegisterEvent(Events.MsgEnterRoom, OnMsgEnterRoom);
        EventManager.Instance.RegisterEvent(Events.MsgLeaveRoom, OnMsgLeaveRoom);
        EventManager.Instance.RegisterEvent(Events.MsgStartBattle, OnMsgStartBattle);
    }

    public void RemoveListener()
    {
        EventManager.Instance.RemoveEvent(Events.MsgEnterRoom, OnMsgEnterRoom);
        EventManager.Instance.RemoveEvent(Events.MsgLeaveRoom, OnMsgLeaveRoom);
        EventManager.Instance.RemoveEvent(Events.MsgStartBattle, OnMsgStartBattle);
    }

    #region 协议事件

    private void OnMsgEnterRoom(MsgBase msgBase)
    {
        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        if (msg.result == 0) // 成功退出房间
        {
            view.DeleteLastGo();
            view.UpdateUIEnterRoom(msg);
        }
        else
            PanelManager.Instance.Open<TipPanel>($"用户进入房间异常");
    }

    /// <summary>
    /// 收到退出房间协议
    /// </summary>
    private void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        Debug.Log($"收到退出房间协议");
        if (msg.result == 0) // 成功退出房间
        {
            if (msg.ID == GameMain.ID)
            {
                Debug.Log($"本人退出房间");
                PanelManager.Instance.Open<RoomHallPanelView>();
                PanelManager.Instance.Open<TipPanel>("退出房间");
                view.OnClose();
            }
            else
            {
                view.DeletePlayerGoByID(msg.ID);
                Debug.Log($"其他玩家退出房间");
            }
        }
        else
            PanelManager.Instance.Open<TipPanel>("退出房间失败");
    }

    /// <summary>
    /// 收到开战协议
    /// </summary>
    private void OnMsgStartBattle(MsgBase msgBase)
    {
        Debug.Log($"收到开战协议");
        MsgStartBattle msg = (MsgStartBattle)msgBase;
        if (msg.result == 0)//开战
        {
            view.OnClose();
        }
        else // 开战失败
            PanelManager.Instance.Open<TipPanel>("开战失败!两队至少都需要一名玩家，只有队长可以开始战斗！");
    }

    #endregion

    #region UI事件

    /// <summary>
    /// 点击开战按钮
    /// </summary>
    public void OnStartClick()
    {
        Debug.Log($"发送开始战斗协议");
        MsgStartBattle msg = new MsgStartBattle();
        NetManager.Send(msg);
    }

    /// <summary>
    /// 点击退出按钮
    /// </summary>
    public void OnCloseClick()
    {
        Debug.Log($"发送退出房间协议");
        MsgLeaveRoom msg = new MsgLeaveRoom() { roomID = this.roomID };
        NetManager.Send(msg);
    }

    #endregion

}