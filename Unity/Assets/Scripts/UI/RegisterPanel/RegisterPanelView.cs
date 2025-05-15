using TMPro;
using UnityEngine.UI;

public class RegisterPanelView : BasePanel
{
    private RegisterPanelController controller;
    public TMP_InputField idInput; // 账号输入框
    public TMP_InputField pwInput; // 密码输入框
    public TMP_InputField repInput; // 重复输入框
    public Button registerBtn; // 注册按钮
    public Button closeBtn; // 关闭按钮

    public override void OnInit() // 初始化
    {
        layer = PanelManager.Layer.Panel;

        // 寻找组件
        idInput = transform.Find("IdInput").GetComponent<TMP_InputField>();
        pwInput = transform.Find("PwInput").GetComponent<TMP_InputField>();
        repInput = transform.Find("RepInput").GetComponent<TMP_InputField>();
        registerBtn = transform.Find("RegisterBtn").GetComponent<Button>();
        closeBtn = transform.Find("CloseBtn").GetComponent<Button>();

        controller = new RegisterPanelController(this);
    }

    public override void OnShow(params object[] para)
    {
        registerBtn.onClick.AddListener(OnRegisterClick);
        closeBtn.onClick.AddListener(OnCloseClick);
    }

    public override void OnClose()
    {
        registerBtn.onClick.RemoveListener(OnRegisterClick);
        closeBtn.onClick.RemoveListener(OnCloseClick);
        gameObject.SetActive(false);
    }

    private void OnRegisterClick()
    {
        controller.OnRegister(idInput.text, pwInput.text, repInput.text);
    }

    private void OnCloseClick()
    {
        OnClose();
    }
}