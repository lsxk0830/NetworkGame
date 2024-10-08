using System;

/// <summary>
/// 玩家
/// </summary>
public class Player
{
    public string id = "";

    /// <summary>
    /// 客户端状态ClientState
    /// </summary>
    public ClientState state;

    /// <summary>
    /// 数据库数据
    /// </summary>
    public PlayerData data;

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

    // 坐标和旋转
    public float x;
    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;
        
    /// <summary>
    /// 在哪个房间
    /// </summary>
    public int roomId = -1;

    /// <summary>
    /// 阵营
    /// </summary>
    public int camp = 1;

    /// <summary>
    /// 坦克生命值
    /// </summary>
    public int hp = 0;
}