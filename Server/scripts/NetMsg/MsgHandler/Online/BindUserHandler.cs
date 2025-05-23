public partial class MsgHandler
{
    /// <summary>
    /// BindUser协议
    /// </summary>
    public static void MsgBindUser(ClientState c, MsgBase msgBase)
    {
        Console.WriteLine("接收:MsgBindUser协议"); ;
        MsgBindUser msg = (MsgBindUser)msgBase;
        UserManager.AddUserCS(msg.ID, c);
    }
}