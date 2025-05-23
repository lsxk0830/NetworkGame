public partial class MsgHandler
{
    /// <summary>
    /// Ping协议处理
    /// </summary>
    public static void MsgPing(ClientState c, MsgBase msgBase)
    {
        Console.WriteLine("接收:MsgPing协议");
        c.lastPingTime = NetManager.GetTimeStamp();
        MsgPong msgPong = new MsgPong();
        NetManager.Send(c, msgPong);
    }
}