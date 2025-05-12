/// <summary>
/// 玩家数据
/// </summary>
public class User
{
    /// <summary>
    /// 用户唯一ID
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 金币数
    /// </summary>
    public int Coin { get; set; }

    /// <summary>
    /// 钻石数
    /// </summary>
    public int Diamond { get; set; }

    /// <summary>
    /// 胜利数
    /// </summary>
    public int Win { get; set; }

    /// <summary>
    /// 失败数
    /// </summary>
    public int Lost { get; set; }

    /// <summary>
    /// 用户头像
    /// </summary>
    public string AvatarPath { get; set; }

    /// <summary>
    /// 上次登录时间
    /// </summary>
    public DateTime LastLogin { get; set; }

    /// <summary>
    /// 胜率
    /// </summary>
    public float WinRate =>
        (Win + Lost) == 0 ? 0 : (float)Win / (Win + Lost) * 100;

    /// <summary>
    /// 添加金币
    /// </summary>
    public void AddCoins(int amount) => Coin = Math.Max(Coin + amount, 0);

    /// <summary>
    /// 消费钻石
    /// </summary>
    public bool SpendDiamonds(int amount)
    {
        if (Diamond < amount) return false;
        Diamond -= amount;
        return true;
    }
}