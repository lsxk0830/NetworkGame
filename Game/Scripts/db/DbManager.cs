using System;
using MySql.Data.MySqlClient;

/// <summary>
/// 用于处理数据库相关事务
/// </summary>
public class DbManager
{
    public static MySqlConnection mysql;

    /// <summary>
    /// 连接MySQL数据库
    /// </summary>
    /// <param name="db"></param>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="user"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
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
}