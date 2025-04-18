/// <summary>
/// Ping协议
/// </summary>
public class MsgPing : MsgBase
{
    public MsgPing() { protoName = "MsgPing"; }
}

/// <summary>
/// Pong协议
/// </summary>
public class MsgPong : MsgBase
{
    public MsgPong() { protoName = "MsgPong"; }
}