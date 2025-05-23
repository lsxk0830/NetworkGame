using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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
        roomListContent = transform.Find("ListPanel/ScrollView/Viewport/Content");
        roomPrefab = transform.Find("Room").gameObject;
        Controller = new RoomHallPanelController(this);
    }

    public override void OnShow(params object[] args)
    {
        if (args.Length > 0)
        {
            Room[] rooms = (Room[])args;
            DeleteLastGo();
            LoadNowGo(rooms);
        }
        else
            DeleteLastGo();
        createButton.onClick.AddListener(OnCreateRoomClick);
        refreshButton.onClick.AddListener(OnRefreshClick);
        Controller.Addlistener();
    }

    public override void OnClose()
    {
        createButton.onClick.RemoveListener(OnCreateRoomClick);
        refreshButton.onClick.RemoveListener(OnRefreshClick);
        Controller.Removelistener();
    }

    #endregion

    #region 更新面板对象

    /// <summary>
    /// 删除上次的房间列表对象
    /// </summary>
    public void DeleteLastGo()
    {
        foreach (Transform child in roomListContent)
        {
            if (child.gameObject != roomPrefab)
                Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 加载这次的房间列表对象
    /// </summary>
    public void LoadNowGo(Room[] rooms)
    {
        foreach (var room in rooms)
        {
            var item = Instantiate(roomPrefab, roomListContent);
            item.SetActive(true);

            var texts = item.GetComponentsInChildren<TMP_Text>();
            texts[0].text = room.RoomID;
            texts[1].text = $"{room.PlayerCount}/4";
            texts[2].text = room.status == 0 ? "等待中" : "战斗中";

            var button = item.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => OnRoomItemClick(room.RoomID));
        }
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

    #endregion

}