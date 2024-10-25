/// <summary>
/// 登录协议
/// </summary>
public class MsgLogin : MsgBase
{
    public MsgLogin() { protoName = "MsgLogin"; }

    //客户端发
    public string id = "";

    public string pw = "";

    //服务端回（0-成功，1-失败）
    public int result = 0;
}
