using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

/// <summary>
/// 用于处理数据库相关事务
/// 连接MySQL数据库、Register、创建角色、获取玩家数据、更新角色数据、检测用户名密码
/// </summary>
public class DbManager
{
    /// <summary>
    /// 数据库连接对象
    /// </summary>
    public static MySqlConnection mysql;

    /// <summary>
    /// 连接MySQL数据库
    /// </summary>m>
    public static bool Connect(string db, string ip, int port, string user, string pw)
    {
        mysql = new MySqlConnection();
        string s = $"Database={db};Data Source={ip};port={port};User Id={user};Password={pw}";
        mysql.ConnectionString = s;

        try // 连接
        {
            mysql.Open();
            Console.WriteLine("[数据库] Connect succ");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] Connect fail," + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 注册
    /// </summary>
    public static bool Register(string id, string pw)
    {
        if (!DbManager.IsSafeString(id))
        {
            Console.WriteLine("[数据库] Register fail, id not safe");
            return false;
        }
        if (!DbManager.IsSafeString(pw))
        {
            Console.WriteLine("[数据库] Register fail, pw not safe");
            return false;
        }
        if (!IsAccountExist(id)) //能否注册
        {
            Console.WriteLine("[数据库] Register fail, id exist");
            return false;
        }
        //写入数据库User表
        string sql = string.Format("insert into account set id ='{0}' ,pw ='{1}';", id, pw);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] Register fail " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    public static bool CreatePlayer(string id)
    {
        if (!DbManager.IsSafeString(id))
        {
            Console.WriteLine("[数据库] CreatePlayer fail, id not safe");
            return false;
        }
        PlayerData playerData = new PlayerData(); //序列化
        string data = JsonConvert.SerializeObject(playerData);
        //写入数据库
        string sql = string.Format("insert into player set id ='{0}' ,data ='{1}';", id, data);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] CreatePlayer err, " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 获取玩家数据
    /// </summary>
    public static PlayerData GetPlayerData(string id)
    {
        if (!DbManager.IsSafeString(id))
        {
            Console.WriteLine("[数据库] GetPlayerData fail, id not safe");
            return null;
        }
        //sql
        string sql = string.Format("select * from player where id ='{0}';", id);
        try
        {
            //查询
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            if (!dataReader.HasRows)
            {
                dataReader.Close();
                return null;
            }
            //读取
            dataReader.Read();
            string data = dataReader.GetString("data");
            //反序列化
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(data);
            dataReader.Close();
            return playerData;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] GetPlayerData fail, " + e.Message);
            return null;
        }
    }

    /// <summary>
    /// 检测用户名密码
    /// </summary>
    public static bool CheckPassword(string id, string pw)
    {
        if (!DbManager.IsSafeString(id) || !DbManager.IsSafeString(pw))
        {
            Console.WriteLine("[数据库] CheckPassword fail, id or pw not safe");
            return false;
        }
        //查询
        string sql = string.Format("select * from account where id='{0}' and pw='{1}';", id, pw);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] CheckPassword err, " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 更新角色数据
    /// </summary>
    public static bool UpdatePlayerData(string id, PlayerData playerData)
    {
        string data = JsonConvert.SerializeObject(playerData);
        string sql = string.Format("update player set data='{0}' where id ='{1}';", data, id);
        try //更新
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] UpdatePlayerData err, " + e.Message);
            return false;
        }
    }

    #region 私有方法 防sql注入、是否存在该用户、检测用户名密码

    /// <summary>
    /// 判定安全字符串,防sql注入
    /// </summary>
    private static bool IsSafeString(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
    }

    /// <summary>
    /// 是否存在该用户
    /// </summary>
    private static bool IsAccountExist(string id)
    {
        if (!DbManager.IsSafeString(id))
            return false;
        string s = string.Format("select * from account where id='{0}';", id); //sql语句
        try //查询
        {
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return !hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] IsSafeString err, " + e.Message);
            return false;
        }
    }

    #endregion 私有方法 防sql注入、是否存在该用户、检测用户名密码
}