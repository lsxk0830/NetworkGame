using System;

/// <summary>
/// 玩家
/// </summary>
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

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="msgBase">协议</param>
    public void Send(MsgBase msgBase)
    {
        NetManager.Send(state, msgBase);
    }
}