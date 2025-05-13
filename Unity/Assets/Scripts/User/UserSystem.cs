using System.Collections.Generic;

public class UserSystem : Singleton<UserSystem>
{
    public Dictionary<long, User> Users;

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
}