/// <summary>
/// 玩家信息
/// </summary>
[Serializable]
public class PlayerInfo
{
    /// <summary>
    /// 账号ID
    /// </summary>
    public string id = "";

    /// <summary>
    /// 阵营 1-第一个阵营 2-第二个阵营
    /// </summary>
    public int camp = 0;

    /// <summary>
    /// 胜利数
    /// </summary>
    public int win = 0;

    /// <summary>
    /// 失败数
    /// </summary>
    public int lost = 0;

    /// <summary>
    /// 是否是房主 1-房主 0-普通成员
    /// </summary>
    public int isOwner = 0;
}