using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HomePanelController
{
    private readonly HomePanelModel model;
    private readonly HomePanelView view;

    public HomePanelController(HomePanelView view)
    {
        model = new HomePanelModel();
        this.view = view;
        model.ID = GameMain.ID;
    }

    public void UpdateUI()
    {
        view.UpdateUserInfo(model.GetUser());
        if (model.GetUser().AvatarPath != "defaultAvatar")
            HTTPManager.Instance.SetAvatar(model.GetUser().AvatarPath, view.GetAvatarImage()).Forget();
        //NetManager.Send(new MsgGetRoomList());
    }

    #region 网络响应处理

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
                PanelManager.Instance.Open<RoomPanel>();
                view.Close();
            },
            fail: () => PanelManager.Instance.Open<TipPanel>("创建房间失败"));
    }

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
        model.isWaitingServerResponse = false;
        view.SetInteractionState(true);

        if (result == 0) success?.Invoke();
        else fail?.Invoke();
    }

    #endregion

    #region 用户操作处理

    public void HandleQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void HandleFace()
    {
        PanelManager.Instance.Open<FacePanelView>();
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
            PanelManager.Instance.Open<TipPanel>("请等待服务器响应");
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

    #region 事件监听

    public void Addlistener()
    {
        EventManager.Instance.RegisterEvent(Events.MsgGetRoomList, HandleRoomListResponse);
        EventManager.Instance.RegisterEvent(Events.MsgCreateRoom, HandleCreateRoomResponse);
        EventManager.Instance.RegisterEvent(Events.MsgEnterRoom, HandleEnterRoomResponse);
    }

    public void Removelistener()
    {
        EventManager.Instance.RemoveEvent(Events.MsgGetRoomList, HandleRoomListResponse);
        EventManager.Instance.RemoveEvent(Events.MsgCreateRoom, HandleCreateRoomResponse);
        EventManager.Instance.RemoveEvent(Events.MsgEnterRoom, HandleEnterRoomResponse);
    }
    #endregion
}