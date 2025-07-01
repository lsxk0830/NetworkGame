public partial class MsgHandler
{
    private static MsgPong msgPong = new MsgPong(); // Pong协议实例

    /// <summary>
    /// Ping协议处理
    /// </summary>
    public static void MsgPing(ClientState c, MsgBase msgBase)
    {
        //Console.WriteLine("接收:MsgPing协议");
        c.lastPingTime = NetManager.GetTimeStamp();
        NetManager.Send(c, msgPong);
    }
}