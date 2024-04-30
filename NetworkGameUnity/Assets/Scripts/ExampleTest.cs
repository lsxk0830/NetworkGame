using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleTest : MonoBehaviour
{

    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
    }

    /// <summary>
    /// 玩家点击连接按钮
    /// </summary>
    public void OnConnectClick()
    {
        NetManager.Connect("127.0.0.1", 8888);
        // TODO:开始转圈圈，提示“连接中”
    }

    /// <summary>
    /// 连接成功回调
    /// </summary>
    private void OnConnectSucc(string err)
    {
        Debug.Log("OnConnectSucc");
        // TODO:进入游戏
    }
    /// <summary>
    /// 连接失败回调
    /// </summary>
    private void OnConnectFail(string err)
    {
        Debug.LogError($"OnConnectFail:{err}");
        // TODO:弹出提示框（连接失败，请重试）
    }
    /// <summary>
    /// 关闭连接
    /// </summary>
    private void OnConnectClose(string err)
    {
        Debug.LogError("OnConnectClose");
        // TODO:弹出提示框（网络断开）
        // TODO:弹出按钮（重新连接）
    }
}
