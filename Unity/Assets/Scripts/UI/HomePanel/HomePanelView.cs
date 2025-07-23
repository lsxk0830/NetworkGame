using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class HomePanelView : BasePanel
{
    public HomePanelController Controller;

    [Header("Text")]
    [SerializeField][LabelText("姓名")] private TMP_Text nameText;
    [SerializeField][LabelText("胜负记录")] private TMP_Text RecordText;
    [SerializeField][LabelText("金币")] private TMP_Text CoinCountText;
    [SerializeField][LabelText("钻石")] private TMP_Text diamondText;

    [Header("Button")]
    [LabelText("头像按钮")] public Button faceBtn;
    [SerializeField][LabelText("退出游戏")] private Button quitBtn;
    [SerializeField][LabelText("开始游戏")] private Button playBtn;

    #region 生命周期
    public override void OnInit()
    {
        layer = PanelManager.Layer.Panel;

        // 寻找组件
        nameText = transform.Find("Top/UserNameText").GetComponent<TMP_Text>();
        RecordText = transform.Find("Top/RecordText").GetComponent<TMP_Text>();
        CoinCountText = transform.Find("Top/CoinCountText").GetComponent<TMP_Text>();
        diamondText = transform.Find("Top/DiamondCountText").GetComponent<TMP_Text>();

        quitBtn = transform.Find("Top/QuitBtn").GetComponent<Button>();
        faceBtn = transform.Find("Top/FaceBtn").GetComponent<Button>();
        playBtn = transform.Find("Down/PlayBtn").GetComponent<Button>();

        Controller = new HomePanelController(this);
        Controller.UpdateUI().Forget();
    }

    public override void OnShow(params object[] args)
    {
        Controller.UpdateUserInfo();
        bool activeMusic = PlayerPrefs.GetInt("Toggle_Music") == 1 ? true : false;
        bool activeSound = PlayerPrefs.GetInt("Toggle_Sound") == 1 ? true : false;
        float m = PlayerPrefs.GetFloat("Slider_Music");
        float s = PlayerPrefs.GetFloat("Slider_Sound");
        BGMusicManager.Instance.ChangeOpen(activeMusic);
        BGMusicManager.Instance.ChangeValue(m);

        gameObject.SetActive(true);
        GameMain.tankModel.SetActive(true);
        Camera.main.transform.SetPositionAndRotation(
           new Vector3(-1, 10, -14),
           Quaternion.Euler(15, 0, 0));

        quitBtn.onClick.AddListener(OnQuitClick);
        faceBtn.onClick.AddListener(OnFaceClick);
        playBtn.onClick.AddListener(OnPlayClick);
        GloablMono.Instance.OnUpdate += OnUpdate;

        EventManager.Instance.RegisterEvent(Events.GoHome, OnGoHome);
    }

    private void OnGoHome()
    {
        playBtn.gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        playBtn.gameObject.SetActive(true);
        gameObject.SetActive(false);
        GameMain.tankModel.SetActive(false);
        quitBtn.onClick.RemoveListener(OnQuitClick);
        faceBtn.onClick.RemoveListener(OnFaceClick);
        playBtn.onClick.RemoveListener(OnPlayClick);
        GloablMono.Instance.OnUpdate -= OnUpdate;
        EventManager.Instance.RemoveEvent(Events.GoHome, OnGoHome);
    }

    #endregion

    #region 数据更新

    public void UpdateUserInfo(User user)
    {
        nameText.text = user.Name;
        RecordText.text = $"{user.Win}胜 >> {user.Lost}负";
        CoinCountText.text = $"{user.Coin}";
        diamondText.text = user.Diamond.ToString();
    }

    #endregion

    #region UI交互

    private void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == GameMain.tankModel)
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
        this.Log("开始游戏");
        playBtn.gameObject.SetActive(false);
        Controller.HandlePlay();
    }

    #endregion

    #region 外部控制
    public void RotateTank(float delta)
    {
        GameMain.tankModel.transform.Rotate(Vector3.up, delta);
    }

    #endregion

    #region UI更新

    public Image GetAvatarImage()
    {
        return faceBtn.GetComponent<Image>();
    }

    #endregion
}