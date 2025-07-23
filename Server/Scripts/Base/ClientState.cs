using System.Net.Sockets;

/// <summary>
/// 客户端信息，每一个客户端连接会对应一个ClientState对象
/// 含有与客户端连接的套接字socket和读缓冲区readBuff
/// </summary>
public class ClientState : IDisposable
{
    public Socket? socket;
    public ByteArray readBuff = new ByteArray();

    /// <summary>
    /// 最后收到Ping的时间
    /// </summary>
    public long lastPingTime = 0;

    /// <summary>
    /// 用户，玩家只有在进入房间后才会有Player对象
    /// </summary>
    public User? user;

    public void Dispose()
    {
        user = null;
        readBuff = null;
    }
}