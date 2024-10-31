/// <summary>
/// 注册协议
/// </summary>
public class MsgRegister : MsgBase
{
    public MsgRegister() { protoName = "MsgRegister"; }

    //客户端发
    public string id = "";

    public string pw = "";

    //服务端回（0-成功，1-失败）
    public int result = 0;
}