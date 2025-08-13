using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Cinemachine;

public class HomePanelView : BasePanel
{
    public HomePanelController Controller;

    [Header("Text")]
    [SerializeField][LabelText("姓名")] private TMP_Text nameText;
    [SerializeField][LabelText("胜负记录")] private TMP_Text RecordText;
    [SerializeField][LabelText("金币")] private TMP_Text CoinCountText;
    [SerializeField][LabelText("钻石")] private TMP_Text diamondText;

    [Header("Button")]
    [SerializeField][LabelText("退出游戏")] private Button QuitBtn;
    [SerializeField][LabelText("设置按钮")] public Button SetBtn;
    [SerializeField][LabelText("头像按钮")] public Button FaceBtn;
    [SerializeField][LabelText("显示按钮")] public Button InfoOpenBtn;
    [SerializeField][LabelText("隐藏按钮")] public Button InfoCloseBtn;
    [SerializeField][LabelText("开始游戏")] private Button PlayBtn;
    [SerializeField][LabelText("签到按钮")] private Button SignInBtn;

    private Camera mainCamera;

    [Header("CMFreeLook")]
    [SerializeField] private CinemachineInputAxisController input;
    [SerializeField]
    private CinemachineInputAxisController Input
    {
        get
        {
            if (input == null)
            {
                input = GameObject.FindWithTag("CMFreeLook").GetComponent<CinemachineInputAxisController>();
            }
            return input;
        }
    }

    #region 生命周期
    public override void OnInit()
    {
        layer = PanelManager.Layer.Panel;

        // 寻找组件
        mainCamera = Camera.main;
        nameText = transform.Find("Top/UserNameText").GetComponent<TMP_Text>();
        RecordText = transform.Find("Top/RecordText").GetComponent<TMP_Text>();
        CoinCountText = transform.Find("Top/CoinCountText").GetComponent<TMP_Text>();
        diamondText = transform.Find("Top/DiamondCountText").GetComponent<TMP_Text>();

        QuitBtn = transform.Find("Top/QuitBtn").GetComponent<Button>();
        SetBtn = transform.Find("Top/SetBtn").GetComponent<Button>();
        FaceBtn = transform.Find("Top/FaceBtn").GetComponent<Button>();
        InfoOpenBtn = transform.Find("Top/InfoOpenBtn").GetComponent<Button>();
        InfoCloseBtn = transform.Find("Top/InfoCloseBtn").GetComponent<Button>();
        PlayBtn = transform.Find("Down/PlayBtn").GetComponent<Button>();
        SignInBtn = transform.Find("Right/CalendarBtn").GetComponent<Button>();

        Controller = new HomePanelController(this);
        Controller.UpdateUI().Forget();
    }

    public override void OnShow(params object[] args)
    {
        Controller.UpdateUserInfo();
        bool activeMusic = PlayerPrefs.GetInt("Toggle_BG") == 1 ? true : false;
        bool activeSound = PlayerPrefs.GetInt("Toggle_Effect") == 1 ? true : false;
        float m = PlayerPrefs.GetFloat("Slider_BG");
        float s = PlayerPrefs.GetFloat("Slider_Effect");
        BGMusicManager.Instance.ChangeOpen(activeMusic);
        BGMusicManager.Instance.ChangeValue(m);

        gameObject.SetActive(true);

        QuitBtn.onClick.AddListener(OnQuitClick);
        SetBtn.onClick.AddListener(OnSetClick);
        FaceBtn.onClick.AddListener(OnFaceClick);
        InfoOpenBtn.onClick.AddListener(OnInfoOpenClick);
        InfoCloseBtn.onClick.AddListener(OnInfoCloseClick);
        PlayBtn.onClick.AddListener(OnPlayClick);
        SignInBtn.onClick.AddListener(OnSignInClick);
        GloablMono.Instance.OnUpdate += OnUpdate;

        EventManager.Instance.RegisterEvent(Events.GoHome, OnGoHome);
        EventManager.Instance.RegisterEvent(Events.UpdateCoinDiamond, OnUpdateCoinDiamond);
    }

    private void OnGoHome()
    {
        PlayBtn.gameObject.SetActive(true);
    }
    private void OnUpdateCoinDiamond()
    {
        User user = UserManager.Instance.GetUser(GameMain.ID);
        CoinCountText.text = $"{user.Coin}";
        diamondText.text = user.Diamond.ToString();
    }


    public override void OnClose()
    {
        PlayBtn.gameObject.SetActive(true);
        gameObject.SetActive(false);
        QuitBtn.onClick.RemoveListener(OnQuitClick);
        SetBtn.onClick.RemoveListener(OnSetClick);
        FaceBtn.onClick.RemoveListener(OnFaceClick);
        InfoOpenBtn.onClick.RemoveListener(OnInfoOpenClick);
        InfoCloseBtn.onClick.RemoveListener(OnInfoCloseClick);
        PlayBtn.onClick.RemoveListener(OnPlayClick);
        SignInBtn.onClick.RemoveListener(OnSignInClick);
        GloablMono.Instance.OnUpdate -= OnUpdate;
        EventManager.Instance.RemoveEvent(Events.GoHome, OnGoHome);
    }

    #endregion

    #region 数据更新

    public void UpdateUserInfo(User user)
    {
        nameText.text = user.Name;
        RecordText.text = $"战绩：{user.Win}胜 >> {user.Lost}负";
        CoinCountText.text = $"{user.Coin}";
        diamondText.text = user.Diamond.ToString();
    }

    #endregion

    #region UI交互

    private void OnUpdate()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            var ray = mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject.name == "TankModel")
            {
                Input.enabled = true;
            }
        }

        if (UnityEngine.Input.GetMouseButtonUp(0))
        {
            Input.enabled = false;
        }
    }

    #endregion

    #region UI事件回调

    private void OnQuitClick() // 退出按钮点击回调
    {
        this.Log("退出游戏");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnSetClick() // 设置按钮点击回调
    {
        this.Log("打开设置");
        PanelManager.Instance.Open<SettingPanel>();
    }

    private void OnFaceClick() // 头像按钮点击回调
    {
        Controller.HandleFace();
    }

    private void OnInfoOpenClick() // 显示按钮点击回调
    {
        this.Log("显示用户信息");
        InfoCloseBtn.gameObject.SetActive(true);
        InfoOpenBtn.gameObject.SetActive(false);
        RecordText.text = "战绩：**********";
    }
    private void OnInfoCloseClick() // 隐藏按钮点击回调
    {
        this.Log("隐藏用户信息");
        InfoCloseBtn.gameObject.SetActive(false);
        InfoOpenBtn.gameObject.SetActive(true);
        User user = UserManager.Instance.GetUser(GameMain.ID);
        RecordText.text = $"战绩：{user.Win}胜 >> {user.Lost}负";
    }
    private void OnPlayClick() // 头像按钮点击回调
    {
        this.Log("开始游戏");
        PlayBtn.gameObject.SetActive(false);
        Controller.HandlePlay();
    }

    private void OnSignInClick() // 签到按钮点击回调
    {
        PanelManager.Instance.Open<DailyRewardsPanel>();
        PlayBtn.gameObject.SetActive(false);
    }

    #endregion

    #region UI更新

    public Image GetAvatarImage()
    {
        return FaceBtn.GetComponent<Image>();
    }

    #endregion
}