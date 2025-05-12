/// <summary>
/// 玩家管理器。是否在线、获取玩家、添加玩家、删除玩家
/// </summary>
public class PlayerManager
{
    // 玩家列表
    private static Dictionary<long, Player> players = new Dictionary<long, Player>();

    /// <summary>
    /// 玩家是否在线
    /// </summary>
    public static bool IsOnline(long id)
    {
        return players.ContainsKey(id);
    }

    /// <summary>
    /// 获取玩家
    /// </summary>
    public static Player GetPlayer(long id)
    {
        if (players.ContainsKey(id))
            return players[id];
        return null;
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    public static void AddPlayer(long id, Player player)
    {
        players.Add(id, player);
    }

    /// <summary>
    /// 删除玩家
    /// </summary>
    public static void RemovePlayer(long id)
    {
        players.Remove(id);
    }

    /// <summary>
    /// 广播消息
    /// </summary>
    public static void Broadcast(MsgBase msg)
    {
        foreach (long id in players.Keys)
        {
            Player player = GetPlayer(id);
            player.Send(msg);
        }
    }
}