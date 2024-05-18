using System.Collections.Generic;

public class PlayerManager
{
    // 玩家列表
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    /// <summary>
    /// 玩家是否在线
    /// </summary>
    public static bool IsOnline(string id)
    {
        return players.ContainsKey(id);
    }

    /// <summary>
    /// 获取玩家
    /// </summary>
    public static Player GetPlayer(string id)
    {
        if (players.ContainsKey(id))
            return players[id];
        return null;
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    public static void AddPlayer(string id, Player player)
    {
        players.Add(id, player);
    }

    /// <summary>
    /// 删除玩家
    /// </summary>
    public static void RemovePlayer(string id)
    {
        players.Remove(id);
    }
}