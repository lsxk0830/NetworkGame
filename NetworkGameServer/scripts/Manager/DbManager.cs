using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

/// <summary>
/// ���ڴ������ݿ��������
/// ����MySQL���ݿ⡢Register��������ɫ����ȡ������ݡ����½�ɫ���ݡ�����û�������
/// </summary>
public class DbManager
{
    /// <summary>
    /// ���ݿ����Ӷ���
    /// </summary>
    public static MySqlConnection mysql;

    /// <summary>
    /// ����MySQL���ݿ�
    /// </summary>m>
    public static bool Connect(string db, string ip, int port, string user, string pw)
    {
        mysql = new MySqlConnection();
        string s = $"Database={db};Data Source={ip};port={port};User Id={user};Password={pw}";
        mysql.ConnectionString = s;

        try // ����
        {
            mysql.Open();
            Console.WriteLine("[���ݿ�] Connect succ");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[���ݿ�] Connect fail," + e.Message);
            return false;
        }
    }

    /// <summary>
    /// ע��
    /// </summary>
    public static bool Register(string id, string pw)
    {
        if (!DbManager.IsSafeString(id))
        {
            Console.WriteLine("[���ݿ�] Register fail, id not safe");
            return false;
        }
        if (!DbManager.IsSafeString(pw))
        {
            Console.WriteLine("[���ݿ�] Register fail, pw not safe");
            return false;
        }
        if (!IsAccountExist(id)) //�ܷ�ע��
        {
            Console.WriteLine("[���ݿ�] Register fail, id exist");
            return false;
        }
        //д�����ݿ�User��
        string sql = string.Format("insert into account set id ='{0}' ,pw ='{1}';", id, pw);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[���ݿ�] Register fail " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// ������ɫ
    /// </summary>
    public static bool CreatePlayer(string id)
    {
        if (!DbManager.IsSafeString(id))
        {
            Console.WriteLine("[���ݿ�] CreatePlayer fail, id not safe");
            return false;
        }
        PlayerData playerData = new PlayerData(); //���л�
        string data = JsonConvert.SerializeObject(playerData);
        //д�����ݿ�
        string sql = string.Format("insert into player set id ='{0}' ,data ='{1}';", id, data);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[���ݿ�] CreatePlayer err, " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// ��ȡ�������
    /// </summary>
    public static PlayerData GetPlayerData(string id)
    {
        if (!DbManager.IsSafeString(id))
        {
            Console.WriteLine("[���ݿ�] GetPlayerData fail, id not safe");
            return null;
        }
        //sql
        string sql = string.Format("select * from player where id ='{0}';", id);
        try
        {
            //��ѯ
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            if (!dataReader.HasRows)
            {
                dataReader.Close();
                return null;
            }
            //��ȡ
            dataReader.Read();
            string data = dataReader.GetString("data");
            //�����л�
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(data);
            dataReader.Close();
            return playerData;
        }
        catch (Exception e)
        {
            Console.WriteLine("[���ݿ�] GetPlayerData fail, " + e.Message);
            return null;
        }
    }

    /// <summary>
    /// ����û�������
    /// </summary>
    public static bool CheckPassword(string id, string pw)
    {
        if (!DbManager.IsSafeString(id) || !DbManager.IsSafeString(pw))
        {
            Console.WriteLine("[���ݿ�] CheckPassword fail, id or pw not safe");
            return false;
        }
        //��ѯ
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
            Console.WriteLine("[���ݿ�] CheckPassword err, " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// ���½�ɫ����
    /// </summary>
    public static bool UpdatePlayerData(string id, PlayerData playerData)
    {
        string data = JsonConvert.SerializeObject(playerData);
        string sql = string.Format("update player set data='{0}' where id ='{1}';", data, id);
        try //����
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[���ݿ�] UpdatePlayerData err, " + e.Message);
            return false;
        }
    }

    #region ˽�з��� ��sqlע�롢�Ƿ���ڸ��û�������û�������

    /// <summary>
    /// �ж���ȫ�ַ���,��sqlע��
    /// </summary>
    private static bool IsSafeString(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
    }

    /// <summary>
    /// �Ƿ���ڸ��û�
    /// </summary>
    private static bool IsAccountExist(string id)
    {
        if (!DbManager.IsSafeString(id))
            return false;
        string s = string.Format("select * from account where id='{0}';", id); //sql���
        try //��ѯ
        {
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return !hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("[���ݿ�] IsSafeString err, " + e.Message);
            return false;
        }
    }

    #endregion ˽�з��� ��sqlע�롢�Ƿ���ڸ��û�������û�������
}