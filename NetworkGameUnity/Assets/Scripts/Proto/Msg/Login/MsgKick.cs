/// <summary>
/// 踢下线协议（服务端推送）
/// </summary>
public class MsgKick : MsgBase
{
    public MsgKick() { protoName = "MsgKick"; }

    /// <summary>
    /// 原因（0-其他人登陆同一账号）
    /// </summary>
    public int reason = 0;
}