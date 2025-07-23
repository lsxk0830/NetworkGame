/// <summary>
/// 踢下线协议（服务端推送）
/// </summary>
public class MsgKick : MsgBase
{
    public MsgKick()
    {
        protoName = "MsgKick";
    }
}