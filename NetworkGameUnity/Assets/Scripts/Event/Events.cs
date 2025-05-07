using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Events", menuName = "坦克游戏事件管理")]
public class Events : ScriptableObject
{
    [LabelText("登录成功事件")] public string OnLoginSuccess = "OnLoginSuccess";


}
