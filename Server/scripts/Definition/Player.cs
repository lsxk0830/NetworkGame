using Newtonsoft.Json;

/// <summary>
/// 玩家
/// </summary>
[Serializable]
public class Player
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long ID;

    public string Name;

    // 坐标和旋转
    public float x;

    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;

    /// <summary>
    /// 炮塔旋转.y
    /// </summary>
    public float turretY = 0;

    /// <summary>
    /// 阵营
    /// </summary>
    public int camp = 1;

    /// <summary>
    /// 坦克生命值
    /// </summary>
    public int hp = 0;

    /// <summary>
    /// 皮肤ID
    /// </summary>
    public int skin;

    /// <summary>
    /// 玩家头像路径
    /// </summary>
    public string AvatarPath;

    public int Win;
    public int Lost;

    /// <summary>
    /// 客户端状态ClientState
    /// </summary>
    [JsonIgnore] public ClientState state;

    public Player(ClientState state)
    {
        this.state = state;
    }
}