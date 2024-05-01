using System;
using UnityEngine;

public class ExampleTest : MonoBehaviour
{

    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgMove", OnMsgMove);

        /*
        MsgMove msgMove = new MsgMove();
        byte[] bs = MsgBase.EncodeName(msgMove);
        int count = 0;
        string name = MsgBase.DecodeName(bs, 0, out count);
        Debug.Log($"Name:{name}");
        Debug.Log($"Count:{count}");
        */
    }

    #region 按钮点击

    /// <summary>
    /// 玩家点击连接按钮
    /// </summary>
    public void OnConnectClick()
    {
        NetManager.Connect("127.0.0.1", 8888);
        // TODO:开始转圈圈，提示“连接中”
    }

    /// <summary>
    /// 主动关闭
    /// </summary>
    public void OnCloseClick()
    {
        NetManager.Close();
    }

    /// <summary>
    /// 玩家点击发送按钮
    /// </summary>
    public void OnMoveClick()
    {
        MsgMove msg = new MsgMove();
        msg.x = 120;
        msg.y = 123;
        msg.z = -6;
        NetManager.Send(msg);
    }

    #endregion 按钮点击

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

    /// <summary>
    /// 收到MsgMove协议
    /// </summary>
    private void OnMsgMove(MsgBase msgBse)
    {
        MsgMove msgMove = (MsgMove)msgBse;
        // 消息处理
        Debug.Log($"OnMsgMove msg.x = {msgMove.x}");
        Debug.Log($"OnMsgMove msg.y = {msgMove.y}");
        Debug.Log($"OnMsgMove msg.z = {msgMove.z}");
    }
}
