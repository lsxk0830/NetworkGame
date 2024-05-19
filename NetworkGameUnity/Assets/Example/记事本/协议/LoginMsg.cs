/// <summary>
/// 注册协议
/// </summary>
public class MsgRegister : MsgBase
{
    public MsgRegister()
    {
        protoName = "MsgRegister";
    }

    //客户端发
    public string id = "";

    public string pw = "";

    //服务端回（0-成功，1-失败）
    public int result = 0;
}

/// <summary>
/// 登陆协议
/// </summary>
public class MsgLogin : MsgBase
{
    public MsgLogin()
    {
        protoName = "MsgLogin";
    }

    //客户端发
    public string id = "";

    public string pw = "";

    //服务端回（0-成功，1-失败）
    public int result = 0;
}

/// <summary>
/// 踢下线协议（服务端推送）
/// </summary>
public class MsgKick : MsgBase
{
    public MsgKick()
    {
        protoName = "MsgKick";
    }

    /// <summary>
    /// 原因（0-其他人登陆同一账号）
    /// </summary>
    public int reason = 0;
}