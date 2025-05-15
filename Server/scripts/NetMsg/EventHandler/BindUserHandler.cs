public partial class MsgHandler
{
    /// <summary>
    /// BindUser协议
    /// </summary>
    public static void MsgBindUser(ClientState c, MsgBase msgBase)
    {
        Console.WriteLine("登录消息"); ;
        MsgBindUser msg = new MsgBindUser();
        UserManager.AddUserCS(msg.ID, c);
    }
}