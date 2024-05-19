/// <summary>
/// 玩家攻击协议
/// </summary>
public class MsgAttack : MsgBase
{
    public MsgAttack()
    {
        protoName = "MsgAttack";
    }

    public string desc = "127.0.0.1:6543";
}

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