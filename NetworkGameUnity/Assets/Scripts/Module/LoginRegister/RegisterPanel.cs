using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    public TMP_InputField idInput; // 账号输入框
    public TMP_InputField pwInput; // 密码输入框
    public TMP_InputField repInput; // 重复输入框
    public Button registerBtn; // 注册按钮
    public Button closeBtn; // 关闭按钮

    public override void OnInit() // 初始化
    {
        panelName = "RegisterPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para) // 显示
    {
        // 寻找组件
        idInput = go.transform.Find("IdInput").GetComponent<TMP_InputField>();
        pwInput = go.transform.Find("PwInput").GetComponent<TMP_InputField>();
        repInput = go.transform.Find("RepInput").GetComponent<TMP_InputField>();
        registerBtn = go.transform.Find("RegisterBtn").GetComponent<Button>();
        closeBtn = go.transform.Find("CloseBtn").GetComponent<Button>();
        // 监听
        registerBtn.onClick.AddListener(OnRegisterClick);
        closeBtn.onClick.AddListener(OnCloseClick);

        EventSystem.RegisterEvent(Events.MsgRegister, OnMsgRegister);
    }

    public override void OnClose() // 关闭
    {
        EventSystem.RemoveEvent(Events.MsgRegister, OnMsgRegister);
    }

    private void OnRegisterClick()
    {
        if (idInput.text == "" || pwInput.text == "")
        {
            PanelManager.Open<TipPanel>("用户名和密码不能为空");
            return;
        }
        if (pwInput.text != repInput.text)
        {
            PanelManager.Open<TipPanel>("两次输入的密码不相同");
            return;
        }
        MsgRegister msgRegister = new MsgRegister()
        {
            id = idInput.text,
            pw = pwInput.text
        };
        NetManager.Send(msgRegister);
    }

    private void OnCloseClick()
    {
        Close();
    }


    private void OnMsgRegister(MsgBase msgBse)
    {
        MsgRegister msg = (MsgRegister)msgBse;
        if (msg.result == 0)
        {
            Debug.Log("注册成功");
            PanelManager.Open<TipPanel>("注册成功");
            Close();
        }
        else
            PanelManager.Open<TipPanel>("注册失败");
    }
}