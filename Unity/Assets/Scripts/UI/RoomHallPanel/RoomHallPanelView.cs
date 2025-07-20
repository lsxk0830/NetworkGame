using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 房间大厅面板
/// </summary>
public class RoomHallPanelView : BasePanel
{
    public RoomHallPanelController Controller;

    [Header("物体")]
    [SerializeField][LabelText("房间列表")] private GameObject ListPanelGo;
    [SerializeField][LabelText("进入房间？")] private GameObject CtrlPanelGo;

    [SerializeField] private Button createButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomPrefab;

    #region 生命周期

    public override void OnInit()
    {
        layer = PanelManager.Layer.Panel;
        ListPanelGo = transform.Find("ListPanel").gameObject;
        CtrlPanelGo = transform.Find("CtrlPanel").gameObject;
        createButton = transform.Find("CtrlPanel/CreateBtn").GetComponent<Button>();
        refreshButton = transform.Find("CtrlPanel/ReflashBtn").GetComponent<Button>();
        leaveButton = transform.Find("CtrlPanel/LeaveBtn").GetComponent<Button>();
        roomListContent = transform.Find("ListPanel/ScrollView/Viewport/Content");
        roomPrefab = transform.Find("Room").gameObject;
        Controller = new RoomHallPanelController(this);
    }

    public override void OnShow(params object[] args)
    {
        gameObject.SetActive(true);

        DeleteLastGo();

        createButton.onClick.AddListener(OnCreateRoomClick);
        refreshButton.onClick.AddListener(OnRefreshClick);
        leaveButton.onClick.AddListener(OnLeaveClick);
        Controller.Addlistener();

        NetManager.Instance.Send(new MsgGetRooms());
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
        createButton.onClick.RemoveListener(OnCreateRoomClick);
        refreshButton.onClick.RemoveListener(OnRefreshClick);
        leaveButton.onClick.RemoveListener(OnLeaveClick);
        Controller.Removelistener();
    }

    #endregion

    #region 更新面板对象

    /// <summary>
    /// 删除指定房间对象
    /// </summary>
    public void DeleteGo(string roomID)
    {
        for (int i = 0; i < roomListContent.childCount; i++)
        {
            if (roomListContent.GetChild(i).name == roomID)
            {
                Destroy(roomListContent.GetChild(i).gameObject);
                break;
            }
        }
    }

    /// <summary>
    /// 删除上次的房间列表对象
    /// </summary>
    public void DeleteLastGo()
    {
        for (int i = roomListContent.childCount - 1; i >= 0; i--)
        {
            Destroy(roomListContent.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 加载这次的房间列表对象
    /// </summary>
    public void LoadNowGo(Room[] rooms)
    {
        foreach (var room in rooms)
        {
            if (room.status == 1) continue; // 跳过战斗中的房间
            LoadOneRoom(room);
        }
    }

    public void LoadOneRoom(Room room)
    {
        var item = Instantiate(roomPrefab, roomListContent);
        item.name = room.RoomID;
        item.SetActive(true);

        item.transform.Find("IdText").GetComponent<TextMeshProUGUI>().text = room.RoomID;
        item.transform.Find("CountText").GetComponent<TextMeshProUGUI>().text = $"{room.playerIds.Count}人";

        var button = item.GetComponentInChildren<Button>();
        button.onClick.AddListener(() => OnRoomItemClick(room.RoomID));
    }

    #endregion

    #region UI事件回调

    /// <summary>
    /// 创建房间
    /// </summary>
    private void OnCreateRoomClick()
    {
        Controller.HandleCreateRoom();
    }

    /// <summary>
    /// 刷新面板
    /// </summary>
    private void OnRefreshClick()
    {
        Controller.HandleRefreshRooms();
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    private void OnRoomItemClick(string roomId)
    {
        Controller.HandleJoinRoom(roomId);
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    private void OnLeaveClick()
    {
        OnClose();
        EventManager.Instance.InvokeEvent(Events.GoHome);
    }

    #endregion

}