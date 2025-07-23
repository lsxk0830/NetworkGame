/// <summary>
/// BindUser协议
/// </summary>
public class MsgBindUser : MsgBase
{
    public MsgBindUser()
    {
        protoName = "MsgBindUser";
    }

    public long ID;
}