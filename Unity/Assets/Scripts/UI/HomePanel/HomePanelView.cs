using UnityEngine;
using TMPro;
using UnityEngine.UI;
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
    [LabelText("头像按钮")] public Button faceBtn;
    [SerializeField][LabelText("退出游戏")] private Button quitBtn;
    [SerializeField][LabelText("开始游戏")] private Button playBtn;

    [Header("场景对象")]
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
        playBtn = transform.Find("Down/PlayBtn").GetComponent<Button>();

        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        tankModel = Array.Find(rootObjects, obj => obj.name == "TankA");
        mainCamera = Camera.main;
        Controller = new HomePanelController(this);
        Controller.UpdateUI().Forget();
    }

    public override void OnShow(params object[] args)
    {
        gameObject.SetActive(true);
        tankModel.SetActive(true);
        mainCamera.transform.SetPositionAndRotation(
            new Vector3(-1, 10, -14),
            Quaternion.Euler(15, 0, 0));

        quitBtn.onClick.AddListener(OnQuitClick);
        faceBtn.onClick.AddListener(OnFaceClick);
        playBtn.onClick.AddListener(OnPlayClick);
        GloablMono.Instance.OnUpdate += OnUpdate;
    }

    public override void OnClose()
    {
        gameObject.SetActive(false);
        tankModel.SetActive(false);
        quitBtn.onClick.RemoveListener(OnQuitClick);
        faceBtn.onClick.RemoveListener(OnFaceClick);
        playBtn.onClick.RemoveListener(OnPlayClick);
        GloablMono.Instance.OnUpdate -= OnUpdate;
    }

    #endregion

    #region 数据更新

    public void UpdateUserInfo(User user)
    {
        nameText.text = user.Name;
        scoreText.text = $"{user.Win}胜 >> {user.Lost}负";
        diamondText.text = user.Diamond.ToString();
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
        Controller.HandleFace();
    }
    private void OnPlayClick() // 头像按钮点击回调
    {
        Controller.HandlePlay();
    }

    #endregion

    #region 外部控制
    public void RotateTank(float delta)
    {
        tankModel.transform.Rotate(Vector3.up, delta);
    }

    #endregion

    #region UI更新

    public Image GetAvatarImage()
    {
        return faceBtn.GetComponent<Image>();
    }

    #endregion
}