using Newtonsoft.Json;
using UnityEngine;

public class RegisterPanelController
{
    private RegisterPanelView view;

    public RegisterPanelController(RegisterPanelView view)
    {
        this.view = view;
    }

    public void OnRegister(string Name, string PW, string RePW)
    {
        if (Name == "" || PW == "")
        {
            PanelManager.Instance.Open<TipPanel>("用户名和密码不能为空");
            return;
        }
        if (PW != RePW)
        {
            PanelManager.Instance.Open<TipPanel>("两次输入的密码不相同");
            return;
        }
        MsgRegister RegisterData = new MsgRegister()
        {
            Name = Name,
            PW = PW
        };
        HTTPManager.Instance.Post(API.Register, RegisterData, RegisteSuccess, RegisterFail).Forget();
    }

    #region 网络请求

    private void RegisteSuccess(string result)
    {
        this.Log(result);
        if (string.IsNullOrWhiteSpace(result))
        {
            PanelManager.Instance.Open<TipPanel>("服务器异常，返回空数据");
            Debug.LogError($"登录错误:{result}");
        }

        Accept<long> accept = JsonConvert.DeserializeObject<Accept<long>>(result);
        if (accept == null)
        {
            PanelManager.Instance.Open<TipPanel>("服务器异常，返回空数据");
            Debug.LogError($"登录错误:{result}");
            return;
        }
        if (accept.code == 200 && accept.data != -1)
        {
            Debug.Log("注册成功");
            PanelManager.Instance.Open<TipPanel>("注册成功");
            view.OnClose();
        }
    }

    private void RegisterFail(long code, string error)
    {
        switch (code)
        {
            case 400:
                PanelManager.Instance.Open<TipPanel>($"注册失败,错误码:{code},error");
                Debug.LogError($"注册失败,错误码:{code},error");
                break;
            case 401:
                PanelManager.Instance.Open<TipPanel>($"注册失败,错误码:{code},error");
                Debug.LogError($"注册失败,错误码:{code},error");
                break;
            case 429:
                PanelManager.Instance.Open<TipPanel>($"注册失败,错误码:{code},error");
                Debug.LogError($"注册失败,错误码:{code},error");
                break;
            case 500:
                PanelManager.Instance.Open<TipPanel>($"注册失败,错误码:{code},error");
                Debug.LogError($"注册失败,错误码:{code},error");
                break;
            default:
                PanelManager.Instance.Open<TipPanel>($"注册失败,错误码:{code},error");
                Debug.LogError($"注册失败,错误码:{code},error");
                break;
        }
    }

    #endregion
}