using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;

public class DbManager
{
    private static MySqlConnection _connection;
    private const string DefaultAvatar = "default_avatar.png";

    /// <summary>
    /// 数据库连接（使用自动重连）
    /// </summary>
    public static void Connect(string server, string database, uint port, string uid, string password)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = server,
            Database = database,
            Port = port,
            UserID = uid,
            Password = password,
            CharacterSet = "utf8mb4",
            SslMode = MySqlSslMode.Preferred,
            Pooling = true,
            MinimumPoolSize = 5,
            MaximumPoolSize = 100
        };

        _connection = new MySqlConnection(builder.ToString());

        try
        {
            _connection.Open();
            InitializeDatabase();
            Console.WriteLine($"Database connected! Thread: {Environment.CurrentManagedThreadId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection error: {ex.Message}");
        }
    }

    /// <summary>
    /// 初始化数据库结构
    /// </summary>
    private static void InitializeDatabase()
    {
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Account (
                ID BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
                Name VARCHAR(255) UNIQUE NOT NULL,
                PW CHAR(64) NOT NULL COMMENT 'SHA256哈希值',
                Coin INT UNSIGNED DEFAULT 0,
                Diamond INT UNSIGNED DEFAULT 0,
                Win INT UNSIGNED DEFAULT 0,
                Lost INT UNSIGNED DEFAULT 0,
                AvatarPath VARCHAR(255) DEFAULT 'default_avatar.png',
                CreateTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                LastLogin TIMESTAMP NULL,
                INDEX idx_coin (Coin),
                INDEX idx_diamond (Diamond)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
    }

    /// <summary>
    /// 用户注册（返回用户ID）
    /// </summary>
    public static long Register(string name, string password)
    {
        if (!ValidateName(name) || !ValidatePassword(password))
            return -1;

        const string sql = @"
            INSERT INTO Account 
            (Name, PW, Coin, Diamond, AvatarPath)
            VALUES
            (@name, SHA2(@password, 256), 100, 50, @avatar)
            RETURNING ID;";

        try
        {
            using var cmd = new MySqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@avatar", DefaultAvatar);
            return Convert.ToInt64(cmd.ExecuteScalar());
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            Console.WriteLine($"用户名已存在: {name}");
            return -1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"注册失败: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// 用户登录验证（返回完整用户对象）
    /// </summary>
    public static User Login(string name, string password)
    {
        const string sql = @"
            SELECT 
                ID, Name, Coin, Diamond, 
                Win, Lost, AvatarPath, LastLogin
            FROM Account 
            WHERE Name = @name 
              AND PW = SHA2(@password, 256);";

        try
        {
            using var cmd = new MySqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@password", password);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new User
            {
                ID = reader.GetInt64("ID"),
                Name = reader.GetString("Name"),
                Coin = reader.GetInt32("Coin"),
                Diamond = reader.GetInt32("Diamond"),
                Win = reader.GetInt32("Win"),
                Lost = reader.GetInt32("Lost"),
                AvatarPath = reader.GetString("AvatarPath"),
                LastLogin = reader.IsDBNull("LastLogin") ?
                    DateTime.MinValue : reader.GetDateTime("LastLogin")
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"登录失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 更新用户数据（线程安全）
    /// </summary>
    public static bool UpdateUser(User user)
    {
        const string sql = @"
            UPDATE Account 
            SET 
                Coin = @coin,
                Diamond = @diamond,
                Win = @win,
                Lost = @lost,
                AvatarPath = @avatar,
                LastLogin = CURRENT_TIMESTAMP
            WHERE ID = @id;";

        try
        {
            using var cmd = new MySqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@id", user.ID);
            cmd.Parameters.AddWithValue("@coin", user.Coin);
            cmd.Parameters.AddWithValue("@diamond", user.Diamond);
            cmd.Parameters.AddWithValue("@win", user.Win);
            cmd.Parameters.AddWithValue("@lost", user.Lost);
            cmd.Parameters.AddWithValue("@avatar", user.AvatarPath);

            return cmd.ExecuteNonQuery() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"更新失败: {ex.Message}");
            return false;
        }
    }

    #region 验证工具
    private static bool ValidateName(string name)
    {
        return Regex.IsMatch(name, @"^[\p{L}\p{N}]{4,20}$");
    }

    private static bool ValidatePassword(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*[A-Za-z])(?=.*\d).{8,}$");
    }
    #endregion

    #region 执行工具
    private static void ExecuteNonQuery(string sql)
    {
        try
        {
            using var cmd = new MySqlCommand(sql, _connection);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"执行失败: {ex.Message}");
        }
    }
    #endregion
}