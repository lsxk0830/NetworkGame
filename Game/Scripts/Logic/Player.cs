public class Player
{
    public string id = "";
    public ClientState state;

    /// <summary>
    /// 数据库数据
    /// </summary>
    public PlayerData data;

    // 临时数据
    public int x, y, z;

    public Player(ClientState state)
    {
        this.state = state;
    }

    public void Send(MsgBase msgBase)
    {
        NetManager.Send(state, msgBase);
    }
}