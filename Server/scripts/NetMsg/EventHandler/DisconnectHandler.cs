public partial class EventHandler
{
    /// <summary>
    /// 离线协议处理
    /// </summary>
    public static void OnDisconnect(ClientState c)
    {
        Console.WriteLine($"关闭Socket:{c.socket.RemoteEndPoint}");
        UserManager.RemoveUser(c);
    }
}