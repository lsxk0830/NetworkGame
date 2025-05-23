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
        EventManager.Instance.RegisterEvent(Events.MsgEnterRoom, HandleEnterRoomResponse);
    }

    public void Removelistener()
    {
        EventManager.Instance.RemoveEvent(Events.MsgGetRooms, HandleRoomListResponse);
        EventManager.Instance.RemoveEvent(Events.MsgCreateRoom, HandleCreateRoomResponse);
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
        HandleOperationResponse(response.result,
            success: () =>
            {
                PanelManager.Instance.Open<RoomPanel>();
                view.Close();
            },
            fail: () => PanelManager.Instance.Open<TipPanel>("创建房间失败"));
    }

    /// <summary>
    /// 进入房间协议
    /// </summary>
    private void HandleEnterRoomResponse(MsgBase msg)
    {
        var response = (MsgEnterRoom)msg;
        HandleOperationResponse(response.result,
            success: () =>
            {
                PanelManager.Instance.Open<RoomPanel>();
                view.Close();
            },
            fail: () => PanelManager.Instance.Open<TipPanel>("进入房间失败"));
    }

    private void HandleOperationResponse(int result, Action success, Action fail)
    {
        if (result == 0) success?.Invoke();
        else fail?.Invoke();
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