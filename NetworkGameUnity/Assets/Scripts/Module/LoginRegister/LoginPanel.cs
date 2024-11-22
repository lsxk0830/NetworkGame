using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public TMP_InputField idInput; // 账号输入框
    public TMP_InputField pwInput; // 密码输入框
    public Button loginBtn; // 登录按钮
    public Button registerBtn; // 注册按钮
    public Toggle isShowPwToggle; // 是否显示密码

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
        isShowPwToggle = skin.transform.Find("IsShowPwToggle").GetComponent<Toggle>();
        // 监听
        loginBtn.onClick.AddListener(OnLoginClick);
        registerBtn.onClick.AddListener(onRegisterClick);
        isShowPwToggle.onValueChanged.AddListener(OnPwShowChangeClick);

        // 网络协议监听
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        // 网络事件监听
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        // 连接服务器
        NetManager.Connect("127.0.0.1", 8888);
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

    #endregion

    /// <summary>
    /// 收到登录协议
    /// </summary>
    private void OnMsgLogin(MsgBase msgBse)
    {
        MsgLogin msg = (MsgLogin)msgBse;
        if (msg.result == 0)
        {
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
    private void OnConnectSucc(string err)
    {
        Debug.Log($"OnConnectSucc");
    }

    /// <summary>
    /// 连接失败回调
    /// </summary>
    private void OnConnectFail(string err)
    {
        GloablMono.Instance.TriggerFromOtherThread(() =>
        {
            PanelManager.Open<TipPanel>(err,2);
        });
    }
}