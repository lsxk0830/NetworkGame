/// <summary>
/// 玩家数据
/// </summary>
public class PlayerData
{
    /// <summary>
    /// 用户唯一ID
    /// </summary>
    public long ID;

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName = "";

    /// <summary>
    /// 金币数
    /// </summary>
    public int Coin = 0;

    /// <summary>
    /// 钻石数
    /// </summary>
    public int Diamond = 0;

    /// <summary>
    /// 胜利数
    /// </summary>
    public int Win = 0;

    /// <summary>
    /// 失败数
    /// </summary>
    public int Lost = 0;

    /// <summary>
    /// 用户头像
    /// </summary>
    public string AvatarPath;
}