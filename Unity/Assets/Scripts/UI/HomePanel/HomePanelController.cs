using System;
using UnityEngine.UI;
using UnityEngine;

public class HomePanelController
{
    private readonly HomePanelModel model;
    private readonly HomePanelView view;

    public HomePanelController(HomePanelView view)
    {
        model = new HomePanelModel();
        this.view = view;

        EventSystem.RegisterEvent(Events.MsgGetAchieve, HandleAchieveResponse);
        EventSystem.RegisterEvent(Events.MsgGetRoomList, HandleRoomListResponse);
        EventSystem.RegisterEvent(Events.MsgCreateRoom, HandleCreateRoomResponse);
        EventSystem.RegisterEvent(Events.MsgEnterRoom, HandleEnterRoomResponse);

        model.playerID = GameMain.ID;
        view.UpdatePlayerInfo(UserSystem.Instance.GetUser(model.playerID).Name, "0胜 0负");
        NetManager.Send(new MsgGetAchieve());
        NetManager.Send(new MsgGetRoomList());
    }

    #region 网络响应处理
    private void HandleAchieveResponse(MsgBase msg)
    {
        var response = (MsgGetAchieve)msg;
        model.winCount = response.win;
        model.lostCount = response.lost;
        view.UpdatePlayerInfo(UserSystem.Instance.GetUser(model.playerID).Name, $"{model.winCount}胜 >> {model.lostCount}负");
    }

    private void HandleRoomListResponse(MsgBase msg)
    {
        var response = (MsgGetRoomList)msg;
        model.rooms.Clear();
        if (response.rooms != null) model.rooms.AddRange(response.rooms);
        view.UpdateRoomList(model.rooms);
    }

    private void HandleCreateRoomResponse(MsgBase msg)
    {
        var response = (MsgCreateRoom)msg;
        HandleOperationResponse(response.result,
            success: () =>
            {
                PanelManager.Open<RoomPanel>();
                view.Close();
            },
            fail: () => PanelManager.Open<TipPanel>("创建房间失败"));
    }

    private void HandleEnterRoomResponse(MsgBase msg)
    {
        var response = (MsgEnterRoom)msg;
        HandleOperationResponse(response.result,
            success: () =>
            {
                PanelManager.Open<RoomPanel>();
                view.Close();
            },
            fail: () => PanelManager.Open<TipPanel>("进入房间失败"));
    }

    private void HandleOperationResponse(int result, Action success, Action fail)
    {
        model.isWaitingServerResponse = false;
        view.SetInteractionState(true);

        if (result == 0) success?.Invoke();
        else fail?.Invoke();
    }
    #endregion

    #region 用户操作处理
    public void HandleFace()
    {
        PanelManager.Open<FacePanelView>();
    }
    public void HandleCreateRoom()
    {
        if (!ValidateOperation()) return;
        NetManager.Send(new MsgCreateRoom());
        SetWaitingState();
    }

    public void HandleRefreshRooms()
    {
        if (!ValidateOperation()) return;
        NetManager.Send(new MsgGetRoomList());
        SetWaitingState();
    }

    public void HandleJoinRoom(int roomId)
    {
        if (!ValidateOperation()) return;
        NetManager.Send(new MsgEnterRoom { id = roomId });
        SetWaitingState();
    }

    private bool ValidateOperation()
    {
        if (model.isWaitingServerResponse)
        {
            PanelManager.Open<TipPanel>("请等待服务器响应");
            return false;
        }
        return true;
    }

    private void SetWaitingState()
    {
        model.isWaitingServerResponse = true;
        view.SetInteractionState(false);
    }
    #endregion

    #region 坦克控制
    public void StartTankRotation(Vector3 mousePosition)
    {
        model.isRotatingTank = true;
        model.lastMousePosition = mousePosition;
    }

    public void UpdateTankRotation()
    {
        if (!model.isRotatingTank) return;

        var currentPos = Input.mousePosition;
        var deltaX = currentPos.x - model.lastMousePosition.x;
        view.RotateTank(-deltaX * HomePanelModel.TankRotationSpeed * Time.deltaTime);
        model.lastMousePosition = currentPos;
    }

    public void EndTankRotation()
    {
        model.isRotatingTank = false;
    }
    #endregion

    #region 资源清理
    public void Destroy()
    {
        EventSystem.RemoveEvent(Events.MsgGetAchieve, HandleAchieveResponse);
        EventSystem.RemoveEvent(Events.MsgGetRoomList, HandleRoomListResponse);
        EventSystem.RemoveEvent(Events.MsgCreateRoom, HandleCreateRoomResponse);
        EventSystem.RemoveEvent(Events.MsgEnterRoom, HandleEnterRoomResponse);
    }
    #endregion
}