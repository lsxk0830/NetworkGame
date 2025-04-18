using SimpleFrame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public TMP_InputField idInput; // 账号输入框
    public TMP_InputField pwInput; // 密码输入框
    public Button loginBtn; // 登录按钮
    public Button registerBtn; // 注册按钮
    public Button readPwBtn; // 本地读取密码按钮
    public Toggle isShowPwToggle; // 是否显示密码
    public Toggle RememberPwToggle; // 是否记住密码
    private GameObject tipPanel; // 自动输入密码弹窗

    public override void OnInit() // 初始化
    {
        skinPath = "LoginPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para) // 显示
    {
        // 寻找组件
        idInput = skin.transform.Find("IDInput").GetComponent<TMP_InputField>();
        pwInput = skin.transform.Find("PWInput").GetComponent<TMP_InputField>();
        loginBtn = skin.transform.Find("LoginBtn").GetComponent<Button>();
        registerBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();
        tipPanel = skin.transform.Find("TipPanel").gameObject;
        readPwBtn = skin.transform.Find("TipPanel/ReadPwBtn").GetComponent<Button>();
        isShowPwToggle = skin.transform.Find("PWInput/IsShowPwToggle").GetComponent<Toggle>();
        RememberPwToggle = skin.transform.Find("RememberPwToggle").GetComponent<Toggle>();
        // 监听
        loginBtn.onClick.AddListener(OnLoginClick);
        registerBtn.onClick.AddListener(onRegisterClick);
        isShowPwToggle.onValueChanged.AddListener(OnPwShowChangeClick);
        readPwBtn.onClick.AddListener(OnReadPwClick);
        idInput.onEndEdit.AddListener(IdInputEnd); // 用户名输入名结束

        // 网络协议监听
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        // 网络事件监听
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        // 连接服务器
        NetManager.Connect("111.229.57.137", 8888);

        RememberPwToggle.isOn = PlayerPrefs.GetInt("RememberPwToggle") == 0 ? true : false;
        if (RememberPwToggle.isOn)
        {
            string str = PlayerPrefs.GetString("idPw");
            int index = str.IndexOf(',');
            if (index != -1)
            {
                // 获取分隔符前的部分
                idInput.text = str.Substring(0, index);
                // 获取分隔符后的部分
                pwInput.text = str.Substring(index + 1);
                //this.Log($"id:{idInput.text},pw:{pwInput.text}");
            }
        }
    }

    public override void OnClose() // 关闭
    {
        NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
    }

    #region UI事件

    private void OnLoginClick()
    {
        if (idInput.text == "" || pwInput.text == "")
        {
            PanelManager.Open<TipPanel>("用户名和密码不能为空");
            return;
        }
        // 发送
        MsgLogin msgLogin = new MsgLogin()
        {
            id = idInput.text,
            pw = pwInput.text
        };
        NetManager.Send(msgLogin);
    }

    private void onRegisterClick()
    {
        PanelManager.Open<RegisterPanel>();
    }

    private void OnPwShowChangeClick(bool isShow)
    {
        pwInput.contentType = isShow ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        pwInput.ForceLabelUpdate();
    }

    private void IdInputEnd(string arg0)
    {
        if (PlayerPrefs.GetString(idInput.text) == "")
            return;
        tipPanel.SetActive(true);
    }

    private void OnReadPwClick()
    {
        string pw = PlayerPrefs.GetString(idInput.text);
        pwInput.text = pw;
        tipPanel.SetActive(false);
    }

    #endregion

    /// <summary>
    /// 收到登录协议
    /// </summary>
    private void OnMsgLogin(MsgBase msgBse)
    {
        MsgLogin msg = (MsgLogin)msgBse;
        if (msg.result == 0)
        {
            PlayerPrefs.SetInt("RememberPwToggle", RememberPwToggle.isOn ? 0 : 1); // 是否保存密码
            if (RememberPwToggle.isOn)
            {
                PlayerPrefs.SetString("idPw", $"{idInput.text},{pwInput.text}");
            }
            Debug.Log($"收到OnMsgLogin协议:登录成功");
            GameMain.id = msg.id;
            PanelManager.Open<RoomListPanel>();
            // 关闭界面
            Close();
        }
        else
            PanelManager.Open<TipPanel>("登录失败");
    }

    /// <summary>
    /// 连接成功回调
    /// </summary>
    private void OnConnectSucc(string err) { }

    /// <summary>
    /// 连接失败回调
    /// </summary>
    private void OnConnectFail(string err)
    {
        GloablMono.Instance.TriggerFromOtherThread(() =>
        {
            PanelManager.Open<TipPanel>(err);
        });
    }
}