using System;
using UnityEngine;

public class RoomHallPanelController
{
    private readonly RoomHallPanelView view;

    public RoomHallPanelController(RoomHallPanelView view)
    {
        this.view = view;
    }

    #region 事件监听

    public void Addlistener()
    {
        EventManager.Instance.RegisterEvent(Events.MsgGetRooms, HandleRoomListResponse);
        EventManager.Instance.RegisterEvent(Events.MsgCreateRoom, HandleCreateRoomResponse);
        EventManager.Instance.RegisterEvent(Events.MsgDeleteRoom, HandleDeleteRoomResponse);
        EventManager.Instance.RegisterEvent(Events.MsgEnterRoom, HandleEnterRoomResponse);
    }

    public void Removelistener()
    {
        EventManager.Instance.RemoveEvent(Events.MsgGetRooms, HandleRoomListResponse);
        EventManager.Instance.RemoveEvent(Events.MsgCreateRoom, HandleCreateRoomResponse);
        EventManager.Instance.RemoveEvent(Events.MsgDeleteRoom, HandleDeleteRoomResponse);
        EventManager.Instance.RemoveEvent(Events.MsgEnterRoom, HandleEnterRoomResponse);
    }

    #endregion

    #region 网络响应处理

    /// <summary>
    /// 获取房间列表网络协议
    /// </summary>
    private void HandleRoomListResponse(MsgBase msg)
    {
        var response = (MsgGetRooms)msg;
        view.DeleteLastGo();
        view.LoadNowGo(response.rooms);
        Debug.Log($"接收:MsgGetRooms协议,房间数:{response.rooms.Length}");
    }

    /// <summary>
    /// 创建房间协议
    /// </summary>
    private void HandleCreateRoomResponse(MsgBase msg)
    {
        var response = (MsgCreateRoom)msg;
        if (response.result == 0)
        {
            if (response.ID == GameMain.ID) // 自己创建的房间
            {
                PanelManager.Instance.Open<RoomPanelView>(response);
                view.Close();
            }
            else
            {
                view.LoadOneRoom(response.roomID); // 别人创建房间
            }
        }
        else
        {
            PanelManager.Instance.Open<TipPanel>("创建房间失败");
        }
    }

    /// <summary>
    /// 删除房间协议
    /// </summary>
    private void HandleDeleteRoomResponse(MsgBase msg)
    {
        var response = (MsgDeleteRoom)msg;
        if (response.result == 0)
        {
            view.DeleteGo(response.roomID);
        }
        else
        {
            PanelManager.Instance.Open<TipPanel>("接收删除房间协议失败");
        }
    }

    /// <summary>
    /// 进入房间协议
    /// </summary>
    private void HandleEnterRoomResponse(MsgBase msg)
    {
        var response = (MsgEnterRoom)msg;
        if (response.result == 0)
        {
            PanelManager.Instance.Open<RoomPanelView>(response);
            view.Close();
        }
        else
        {
            PanelManager.Instance.Open<TipPanel>("进入房间失败");
        }
    }

    #endregion

    #region UI点击

    /// <summary>
    /// 创建房间
    /// </summary>
    public void HandleCreateRoom()
    {
        NetManager.Send(new MsgCreateRoom());
    }

    /// <summary>
    /// 刷新房间
    /// </summary>
    public void HandleRefreshRooms()
    {
        NetManager.Send(new MsgGetRooms());
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    public void HandleJoinRoom(string roomId)
    {
        NetManager.Send(new MsgEnterRoom { roomID = roomId });
    }

    #endregion
}