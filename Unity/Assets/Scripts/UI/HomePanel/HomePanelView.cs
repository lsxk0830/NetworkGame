using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class HomePanelView : BasePanel
{
    public HomePanelController Controller;

    [Header("Text")]
    [SerializeField][LabelText("姓名")] private TMP_Text nameText;
    [SerializeField][LabelText("金币")] private TMP_Text scoreText;
    [SerializeField][LabelText("钻石")] private TMP_Text diamondText;

    [Header("Button")]
    [SerializeField][LabelText("头像按钮")] private Button faceBtn;
    [SerializeField][LabelText("退出游戏")] private Button quitBtn;

    [SerializeField] private Button createButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomPrefab;

    // 场景对象
    [Header("Scene References")]
    [SerializeField] private GameObject tankModel;
    [SerializeField] private Camera mainCamera;

    #region 生命周期
    public override void OnInit()
    {
        layer = PanelManager.Layer.Panel;

        // 寻找组件
        nameText = transform.Find("Top/UserNameText").GetComponent<TMP_Text>();
        scoreText = transform.Find("Top/RecordText").GetComponent<TMP_Text>();
        diamondText = transform.Find("Top/DiamondCountText").GetComponent<TMP_Text>();

        quitBtn = transform.Find("Top/QuitBtn").GetComponent<Button>();
        faceBtn = transform.Find("Top/FaceBtn").GetComponent<Button>();

        createButton = transform.Find("CtrlPanel/CreateBtn").GetComponent<Button>();
        refreshButton = transform.Find("CtrlPanel/ReflashBtn").GetComponent<Button>();
        roomListContent = transform.Find("ListPanel/ScrollView/Viewport/Content");
        roomPrefab = transform.Find("Room").gameObject;
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        tankModel = Array.Find(rootObjects, obj => obj.name == "TankA");
        mainCamera = Camera.main;
        Controller = new HomePanelController(this);
        Controller.UpdateUI();
    }

    public override void OnShow(params object[] args)
    {
        roomPrefab.SetActive(false);
        tankModel.SetActive(true);
        mainCamera.transform.SetPositionAndRotation(
            new Vector3(-1, 10, -14),
            Quaternion.Euler(15, 0, 0));

        quitBtn.onClick.AddListener(OnQuitClick);
        faceBtn.onClick.AddListener(OnFaceClick);
        createButton.onClick.AddListener(OnCreateRoomClick);
        refreshButton.onClick.AddListener(OnRefreshClick);
        GloablMono.Instance.OnUpdate += OnUpdate;
        Controller.Addlistener();
    }

    public override void OnClose()
    {
        tankModel.SetActive(false);
        faceBtn.onClick.RemoveListener(OnFaceClick);
        createButton.onClick.RemoveListener(OnCreateRoomClick);
        refreshButton.onClick.RemoveListener(OnRefreshClick);
        GloablMono.Instance.OnUpdate -= OnUpdate;
        Controller.Removelistener();
    }
    #endregion

    #region 数据更新
    public void UpdateUserInfo(User user)
    {
        nameText.text = user.Name;
        scoreText.text = $"{user.Win}胜 >> {user.Lost}负";
        diamondText.text = user.Diamond.ToString();
    }

    public void UpdateRoomList(List<RoomInfo> rooms)
    {
        foreach (Transform child in roomListContent)
        {
            if (child.gameObject != roomPrefab)
                Destroy(child.gameObject);
        }
        foreach (var room in rooms)
        {
            var item = Instantiate(roomPrefab, roomListContent);
            item.SetActive(true);

            var texts = item.GetComponentsInChildren<TMP_Text>();
            texts[0].text = room.id.ToString();
            texts[1].text = $"{room.count}/4";
            texts[2].text = room.status == 0 ? "等待中" : "战斗中";

            var button = item.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => OnRoomItemClick(room.id));
        }
    }

    #endregion

    #region UI交互

    private void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == tankModel)
            {
                Controller.StartTankRotation(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Controller.EndTankRotation();
        }

        Controller.UpdateTankRotation();
    }

    #endregion

    #region UI事件回调
    private void OnQuitClick() // 退出按钮点击回调
    {
        this.Log("退出游戏");
        Controller.HandleQuit();
    }
    private void OnFaceClick() // 头像按钮点击回调
    {
        this.Log("头像按钮点击回调");
        Controller.HandleFace();
    }
    private void OnCreateRoomClick()
    {
        Controller.HandleCreateRoom();
    }

    private void OnRefreshClick()
    {
        Controller.HandleRefreshRooms();
    }

    private void OnRoomItemClick(int roomId)
    {
        Controller.HandleJoinRoom(roomId);
    }
    #endregion

    #region 外部控制
    public void RotateTank(float delta)
    {
        tankModel.transform.Rotate(Vector3.up, delta);
    }

    public void SetInteractionState(bool interactable)
    {
        createButton.interactable = interactable;
        refreshButton.interactable = interactable;
    }
    #endregion

    #region UI更新

    public Image GetAvatarImage()
    {
        return faceBtn.GetComponent<Image>(); ;
    }

    #endregion
}