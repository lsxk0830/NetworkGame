/// <summary>
/// 用户管理器。是否用户、获取用户、添加用户、删除用户
/// </summary>
public class UserManager
{
    // 用户列表
    private static Dictionary<long, User> Users = new Dictionary<long, User>();
    private static Dictionary<long, ClientState> UserCSs = new Dictionary<long, ClientState>(); // Socket和用户ID的映射

    /// <summary>
    /// 用户是否在线
    /// </summary>
    public static bool IsOnline(long id) => Users.ContainsKey(id);

    /// <summary>
    /// 获取用户
    /// </summary>
    public static User? GetUser(long id)
    {
        return Users.ContainsKey(id) ? Users[id] : null;
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

    #region 用户上线添加用户

    /// <summary>
    /// 添加用户
    /// </summary>
    public static void AddUser(long id, User user)
    {
        Users.Add(id, user);
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    public static void AddUserCS(long id, ClientState cs)
    {
        UserCSs.Add(id, cs);
    }

    #endregion

    #region 用户离线删除用户

    /// <summary>
    /// 删除用户
    /// </summary>
    public static void RemoveUser(long id)
    {
        if (Users.ContainsKey(id))
            Users.Remove(id);
        if (UserCSs.ContainsKey(id))
            UserCSs.Remove(id);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    public static void RemoveUser(ClientState cs)
    {
        if (cs.user == null) return;
        RemoveUser(cs.user.ID);
    }

    #endregion
}