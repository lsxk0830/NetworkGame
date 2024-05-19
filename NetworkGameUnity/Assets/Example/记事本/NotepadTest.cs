using UnityEngine;
using UnityEngine.UI;

public class NotepadTest : MonoBehaviour
{
	public InputField idInput;
	public InputField pwInput;
	public InputField textInput;

	void Start()
	{
		NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
		NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
		NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);

		NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
		NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
		NetManager.AddMsgListener("MsgKick", OnMsgKick);
		NetManager.AddMsgListener("MsgGetText", OnMsgGetText);
		NetManager.AddMsgListener("MsgSaveText", OnMsgSaveText);
	}

	void Update()
	{
		NetManager.Update();
	}

	#region 玩家点击连接、关闭、注册、登陆、保存按钮
	/// <summary>
	/// 点击连接
	/// </summary>
	public void OnConnectClick()
	{
		NetManager.Connect("127.0.0.1", 8888);
	}

	/// <summary>
	/// 点击关闭
	/// </summary>
	public void OnCloseClick()
	{
		NetManager.Close();
	}

	/// <summary>
	/// 点击注册
	/// </summary>
	public void OnRegisterClick()
	{
		MsgRegister msg = new MsgRegister();
		msg.id = idInput.text;
		msg.pw = pwInput.text;
		NetManager.Send(msg);
	}

	/// <summary>
	/// 点击登陆
	/// </summary>
	public void OnLoginClick()
	{
		MsgLogin msg = new MsgLogin();
		msg.id = idInput.text;
		msg.pw = pwInput.text;
		NetManager.Send(msg);
	}

	/// <summary>
	/// 点击保存
	/// </summary>
	public void OnSaveClick()
	{
		MsgSaveText msg = new MsgSaveText();
		msg.text = textInput.text;
		NetManager.Send(msg);
	}
	#endregion

	#region 连接成功、连接失败、关闭连接
	/// <summary>
	/// 连接成功回调
	/// </summary>
	private void OnConnectSucc(string err)
	{
		Debug.Log("OnConnectSucc");

	}

	/// <summary>
	/// 连接失败回调
	/// </summary>
	private void OnConnectFail(string err)
	{
		Debug.Log("OnConnectFail " + err);
	}

	/// <summary>
	/// 关闭连接
	/// </summary>
	private void OnConnectClose(string err)
	{
		Debug.Log("OnConnectClose");
	}
	#endregion

	#region 收到协议：注册、登陆、被踢、收到记事本文本、保存文本
	/// <summary>
	/// 收到注册协议
	/// </summary>
	private void OnMsgRegister(MsgBase msgBase)
	{
		MsgRegister msg = (MsgRegister)msgBase;
		if (msg.result == 0)
			Debug.Log("注册成功");
		else
			Debug.Log("注册失败");
	}

	/// <summary>
	/// 收到登陆协议
	/// </summary>
	private void OnMsgLogin(MsgBase msgBase)
	{
		MsgLogin msg = (MsgLogin)msgBase;
		if (msg.result == 0)
		{
			Debug.Log("登陆成功");
			//请求记事本文本
			MsgGetText msgGetText = new MsgGetText();
			NetManager.Send(msgGetText);
		}
		else
			Debug.Log("登陆失败");
	}

	/// <summary>
	/// 被踢下线
	/// </summary>
	private void OnMsgKick(MsgBase msgBase)
	{
		Debug.Log("被踢下线");
	}

	/// <summary>
	/// 收到记事本文本协议
	/// </summary>
	private void OnMsgGetText(MsgBase msgBase)
	{
		MsgGetText msg = (MsgGetText)msgBase;
		textInput.text = msg.text;
	}

	/// <summary>
	/// 收到保存文本协议
	/// </summary>
	private void OnMsgSaveText(MsgBase msgBase)
	{
		MsgSaveText msg = (MsgSaveText)msgBase;
		if (msg.result == 0)
			Debug.Log("保存成功");
		else
			Debug.Log("保存失败");
	}
	#endregion
}
