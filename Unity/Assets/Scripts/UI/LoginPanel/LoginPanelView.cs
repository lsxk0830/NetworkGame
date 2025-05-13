using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanelView : BasePanel
{
    private LoginPanelController controller;
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
        layer = PanelManager.Layer.Panel;
        controller = new LoginPanelController(this);

        // 寻找组件
        idInput = transform.Find("IDInput").GetComponent<TMP_InputField>();
        pwInput = transform.Find("PWInput").GetComponent<TMP_InputField>();
        loginBtn = transform.Find("LoginBtn").GetComponent<Button>();
        registerBtn = transform.Find("RegisterBtn").GetComponent<Button>();
        tipPanel = transform.Find("TipPanel").gameObject;
        readPwBtn = transform.Find("TipPanel/ReadPwBtn").GetComponent<Button>();
        isShowPwToggle = transform.Find("IsShowPwToggle").GetComponent<Toggle>();
        RememberPwToggle = transform.Find("RememberPwToggle").GetComponent<Toggle>();
        controller.SetToggleState();
    }

    public override void OnShow(params object[] para) // 显示
    {
        // 添加监听
        loginBtn.onClick.AddListener(OnLoginClick);
        registerBtn.onClick.AddListener(OnRegisterClick);
        isShowPwToggle.onValueChanged.AddListener(OnPwShowChangeClick);
        readPwBtn.onClick.AddListener(OnReadPwClick);
        idInput.onEndEdit.AddListener(IdInputEnd); // 用户名输入名结束
    }

    public override void OnClose() // 关闭
    {
        // 取消监听
        loginBtn.onClick.RemoveListener(OnLoginClick);
        registerBtn.onClick.RemoveListener(OnRegisterClick);
        isShowPwToggle.onValueChanged.RemoveListener(OnPwShowChangeClick);
        readPwBtn.onClick.RemoveListener(OnReadPwClick);
        idInput.onEndEdit.RemoveListener(IdInputEnd);
        // 关闭面板
        gameObject.SetActive(false);
    }

    #region UI事件

    private void OnLoginClick()
    {
        controller.OnLogin(idInput.text, pwInput.text);
    }

    private void OnRegisterClick()
    {
        controller.OnRegister();
    }

    private void OnPwShowChangeClick(bool isShow)
    {
        controller.OnChangePWState(isShow, pwInput);
    }

    private void IdInputEnd(string arg0)
    {
        controller.IdInputEnd(idInput.text);
    }

    private void OnReadPwClick()
    {
        controller.AutoInputPW(idInput.text);
    }

    #endregion

    #region 更新UI显示

    /// <summary>
    /// 更新记住密码Togglr的状态
    /// </summary>
    /// <param name="isOn"></param>
    public void UpdateToggleState(bool isOn)
    {
        RememberPwToggle.isOn = isOn;
    }

    /// <summary>
    /// 更新用户名显示
    /// </summary>
    public void UpdateNameDisplay(string name)
    {
        idInput.text = name;
    }

    /// <summary>
    /// 更新密码显示
    /// </summary>
    public void UpdatePWDisplay(string pw)
    {
        pwInput.text = pw;
    }

    /// <summary>
    /// 更新自动输入密码弹窗显示状态
    /// </summary>
    public void UpdateTipDisplay(bool state)
    {
        tipPanel.SetActive(state);
    }

    #endregion

    #region 获取UI状态

    /// <summary>
    /// 获取记住密码的Toggle的状态
    /// </summary>
    /// <returns></returns>
    public bool GetRememberPwToggleState()
    {
        return RememberPwToggle.isOn;
    }

    /// <summary>
    /// 获取用户名
    /// </summary>
    public string GetName()
    {
        return idInput.text;
    }

    /// <summary>
    /// 获取密码
    /// </summary>
    public string GetPW()
    {
        return pwInput.text;
    }
    #endregion
}