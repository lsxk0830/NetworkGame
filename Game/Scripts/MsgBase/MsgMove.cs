/// <summary>
/// 玩家移动协议
/// </summary>
public class MsgMove : MsgBase
{
    public MsgMove()
    {
        protoName = "MsgMove";
    }

    public int x = 0;
    public int y = 0;
    public int z = 0;
}