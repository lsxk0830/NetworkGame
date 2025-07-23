using UnityEngine;

public class LoginPanelModel
{
    /// <summary>
    /// 是否记住用户名密码的Toggle的状态
    /// </summary>
    public bool RememberPwToggleState
    {
        get
        {
            return PlayerPrefs.GetInt("RememberPwToggle") == 0 ? true : false;
        }
        set
        {
            PlayerPrefs.SetInt("RememberPwToggle", value ? 0 : 1);// 是否保存密码
        }
    }

    /// <summary>
    /// 上一次登录输入的用户名密码
    /// </summary>
    public string LastLoginUser
    {
        get
        {
            return PlayerPrefs.GetString("LastLoginUserPW");
        }
        set
        {
            PlayerPrefs.SetString("LastLoginUserPW", value); // 设置这次登录的用户名密码
        }
    }
}