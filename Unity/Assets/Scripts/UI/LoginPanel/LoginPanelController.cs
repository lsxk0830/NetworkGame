using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class LoginPanelController
{
    private LoginPanelView view;
    private LoginPanelModel model;
    public LoginPanelController(LoginPanelView view)
    {
        this.view = view;
        model = new LoginPanelModel();
    }

    public void SetToggleState()
    {
        bool isOn = model.RememberPwToggleState;
        view.UpdateToggleState(isOn);
        if (isOn)
        {
            string str = model.LastLoginUser;
            int index = str.IndexOf(',');
            if (index != -1)
            {
                view.UpdateNameDisplay(str.Substring(0, index)); // 获取分隔符前的部分
                view.UpdatePWDisplay(str.Substring(index + 1)); // 获取分隔符后的部分
                //this.Log($"id:{idInput.text},pw:{pwInput.text}");
            }
        }
    }

    #region UI事件

    /// <summary>
    /// 点击登录按钮
    /// </summary>
    public void OnLogin(string name, string pw)
    {
        if (name == "" || pw == "")
        {
            PanelManager.Instance.Open<TipPanel>("用户名和密码不能为空");
            return;
        }
        LoginRequestMsgBody LoginData = new LoginRequestMsgBody()
        {
            Name = name,
            PW = pw
        };
        HTTPManager.Instance.Post(API.Login, LoginData, LoginSuccess, LoginFail).Forget();
    }

    /// <summary>
    /// 点击注册按钮
    /// </summary>
    public void OnRegister()
    {
        PanelManager.Instance.Open<RegisterPanelView>();
    }

    /// <summary>
    /// 切换密码显示隐藏状态
    /// </summary>
    public void OnChangePWState(bool isShow, TMP_InputField pwInput)
    {
        pwInput.contentType = isShow ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        pwInput.ForceLabelUpdate();
    }

    /// <summary>
    /// 用户名输入结束，输入上次的密码
    /// </summary>
    public void IdInputEnd(string userName)
    {
        if (PlayerPrefs.GetString(userName) == "") return;
        view.UpdateTipDisplay(true);
    }

    /// <summary>
    /// 用户名输入结束，输入上次的密码
    /// </summary>
    public void AutoInputPW(string userName)
    {
        string pw = PlayerPrefs.GetString(userName);
        view.UpdatePWDisplay(pw);
        view.UpdateTipDisplay(false);
    }

    #endregion

    #region 网络消息回调

    private void LoginSuccess(string result)
    {
        this.Log($"登录回调：{result}");
        if (string.IsNullOrWhiteSpace(result))
        {
            PanelManager.Instance.Open<TipPanel>("服务器异常，返回空数据");
            Debug.LogError($"登录错误:{result}");
        }

        Accept<User> accept = JsonConvert.DeserializeObject<Accept<User>>(result);
        if (accept == null)
        {
            PanelManager.Instance.Open<TipPanel>("服务器异常，返回空数据");
            Debug.LogError($"登录错误:{result}");
            return;
        }
        if (accept.code == 200)
        {
            UserManager.Instance.Init(accept.data);
            GloablMono.Instance.TriggerFromOtherThread(() =>
            {
                PanelManager.Instance.Close<LoginPanelView>();
                PanelManager.Instance.Open<HomePanelView>();
            });
            bool state = view.GetRememberPwToggleState();
            model.RememberPwToggleState = state;
            if (state)
            {
                PlayerPrefs.SetString(view.GetName(), view.GetPW());
                model.LastLoginUser = $"{view.GetName()},{view.GetPW()}";
            }
            MsgBindUser msg = new MsgBindUser()
            {
                ID = accept.data.ID
            };
            NetManager.Send(msg);
        }
    }

    private void LoginFail(long code, string error)
    {
        switch (code)
        {
            case 400:
                PanelManager.Instance.Open<TipPanel>("请求格式错误");
                Debug.LogError("请求格式错误");
                break;
            case 401:
                PanelManager.Instance.Open<TipPanel>("用户名或密码错误");
                Debug.LogError("用户名或密码错误");
                break;
            case 429:
                PanelManager.Instance.Open<TipPanel>("尝试次数过多，请稍后再试");
                Debug.LogError("尝试次数过多，请稍后再试");
                break;
            case 500:
                PanelManager.Instance.Open<TipPanel>("服务器开小差了，请联系开发人员");
                Debug.LogError("服务器开小差了，请联系开发人员");
                break;
            default:
                PanelManager.Instance.Open<TipPanel>($"连接失败: {error}");
                Debug.LogError($"连接失败: {error}");
                break;
        }
    }

    #endregion
}