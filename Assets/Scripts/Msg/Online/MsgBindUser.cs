/// <summary>
/// Socket绑定用户
/// </summary>
public class MsgBindUser : MsgBase
{
    public MsgBindUser()
    {
        protoName = "MsgBindUser";
    }

    public long ID;
}