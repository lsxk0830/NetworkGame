using System.Collections.Generic;

/// <summary>
/// 用户系统，主要存放所有用户的信息
/// </summary>
public class UserManager : Singleton<UserManager>
{
    private Dictionary<long, User> Users;

    public void Init(User user)
    {
        Users = new Dictionary<long, User>()
        {
            {user.ID,user}
        };
    }

    public void AddUser(User user)
    {
        if (Users.ContainsKey(user.ID))
            Users[user.ID] = user;
        else
            Users.Add(user.ID, user);
    }

    public void RemoveUser(User user)
    {
        if (Users.ContainsKey(user.ID))
            Users.Remove(user.ID);
    }

    public void RemoveUser(long ID)
    {
        if (Users.ContainsKey(ID))
            Users.Remove(ID);
    }

    public User GetUser(long ID)
    {
        if (Users.ContainsKey(ID))
            return Users[ID];
        return null;
    }
}