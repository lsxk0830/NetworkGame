using System.Collections.Generic;

/// <summary>
/// 玩家管理器。是否在线、获取玩家、添加玩家、删除玩家
/// </summary>
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

    /// <summary>
    /// 广播消息
    /// </summary>
    public static void Broadcast(MsgBase msg)
    {
        foreach (string id in players.Keys)
        {
            Player player = GetPlayer(id);
            player.Send(msg);
        }
    }
}