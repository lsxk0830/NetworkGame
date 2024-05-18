using System;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

/// <summary>
/// 用于处理数据库相关事务
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
        string s = string.Format("Database={0};Data Source ={1 };port={2};User Id ={3};Password = {4}", db, ip, port, user, pw);
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
    /// 判定安全字符串,防sql注入
    /// </summary>
    private static bool IsSafeString(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
    }

    /// <summary>
    /// 是否存在该用户
    /// </summary>
    public static bool IsAccountExist(string id)
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
}