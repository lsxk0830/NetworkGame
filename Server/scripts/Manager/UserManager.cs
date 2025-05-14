/// <summary>
/// 用户管理器。是否用户、获取用户、添加用户、删除用户
/// </summary>
public class UserManager
{
    // 玩家列表
    private static Dictionary<long, User> Users = new Dictionary<long, User>();
    private static Dictionary<long, ClientState> UserCSs = new Dictionary<long, ClientState>();

    /// <summary>
    /// 玩家是否在线
    /// </summary>
    public static bool IsOnline(long id) => Users.ContainsKey(id);

    /// <summary>
    /// 获取玩家
    /// </summary>
    public static User? GetUser(long id)
    {
        return Users.ContainsKey(id) ? Users[id] : null;
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    public static void AddUser(long id, User user)
    {
        Users.Add(id, user);
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    public static void AddUserCS(long id, ClientState cs)
    {
        UserCSs.Add(id, cs);
    }

    /// <summary>
    /// 删除玩家
    /// </summary>
    public static void RemoveUser(long id)
    {
        if (Users.ContainsKey(id))
            Users.Remove(id);
        if (UserCSs.ContainsKey(id))
            UserCSs.Remove(id);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    public static void Send(long ID, MsgBase msgBase)
    {
        if (Users.ContainsKey(ID))
        {
            NetManager.Send(UserCSs[ID], msgBase);
        }
    }
}