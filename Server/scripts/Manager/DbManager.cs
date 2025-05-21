using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

public class DbManager
{
    private static MySqlConnection? connection;
    private const string DefaultAvatar = "defaultAvatar"; // 默认头像路径
    private const int DefaultCoin = 100; // 默认金币数
    private const int DefaultDiamond = 100; // 默认钻石数

    /// <summary>
    /// 数据库连接（使用自动重连）
    /// </summary>
    public static bool Connect(string database, string server, uint port, string uid, string password)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = server,
            Port = port,
            Database = database,
            UserID = uid,
            Password = password,
            CharacterSet = "utf8mb4",
            SslMode = MySqlSslMode.Preferred,
            Pooling = true,
            MinimumPoolSize = 5,
            MaximumPoolSize = 100
        };

        connection = new MySqlConnection(builder.ToString());

        try
        {
            connection.Open();
            InitializeDatabase();
            Console.WriteLine($"数据库连接成功! 线程: {Environment.CurrentManagedThreadId}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"数据库连接错误: {ex.Message}");
            return false;
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
                Name VARCHAR(100) UNIQUE NOT NULL  COMMENT'用户名',
                PW CHAR(64) NOT NULL COMMENT '用户密码,SHA256哈希值',
                Coin INT UNSIGNED  NOT NULL  COMMENT'金币数',
                Diamond INT UNSIGNED  NOT NULL COMMENT'钻石数',
                Win INT UNSIGNED DEFAULT 0  COMMENT'胜利局数',
                Lost INT UNSIGNED DEFAULT 0  COMMENT'失败局数',
                AvatarPath VARCHAR(255)  NOT NULL COMMENT'默认头像路径',
                CreateTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT'创建用户时间',
                LastLogin TIMESTAMP NULL COMMENT'上次登录时间',
                INDEX idx_coin (Coin),
                INDEX idx_diamond (Diamond)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
    }

    /// <summary>
    /// 用户注册（返回用户ID）
    /// </summary>
    public static long Register(string name, string password, int coin = DefaultCoin, int diamond = DefaultDiamond, string avatar = DefaultAvatar)
    {
        if (!DbManager.ValidateName(name))
        {
            Console.WriteLine("数据库注册失败, 用户名不安全");
            return -1;
        }
        if (!DbManager.ValidatePassword(password))
        {
            Console.WriteLine("数据库注册失败, 密码不安全");
            return -1;
        }

        const string sql = @"
            INSERT INTO Account
            (Name, PW, Coin, Diamond, AvatarPath)
            VALUES
            (@name, SHA2(@password, 256), @coin, @diamond, @avatar);
            SELECT LAST_INSERT_ID();";

        try
        {
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@coin", coin);
            cmd.Parameters.AddWithValue("@diamond", diamond);
            cmd.Parameters.AddWithValue("@avatar", avatar);

            // 执行查询并获取结果
            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt64(result) : -1;
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
                Win, Lost, AvatarPath, CreateTime, LastLogin
            FROM Account
            WHERE Name = @name
              AND PW = SHA2(@password, 256);";

        try
        {
            User user = null;
            using (var cmd = new MySqlCommand(sql, connection))// 分离读取和更新操作
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@password", password);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            ID = reader.GetInt64("ID"),
                            Name = reader.GetString("Name"),
                            Coin = reader.GetInt32("Coin"),
                            Diamond = reader.GetInt32("Diamond"),
                            Win = reader.GetInt32("Win"),
                            Lost = reader.GetInt32("Lost"),
                            AvatarPath = reader.GetString("AvatarPath"),
                            CreateTime = reader.GetDateTime("CreateTime"),
                            LastLogin = DateTime.Now
                        };
                    }
                }
            }
            if (user != null)
            {
                Console.WriteLine($"成功获取用户信息");
                UpdateUserLastLogin(user);
            }
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"登录失败或更新失败: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 更新用户登录时间
    /// </summary>
    private static void UpdateUserLastLogin(User user)
    {
        const string sql = @" UPDATE Account SET LastLogin = CURRENT_TIMESTAMP WHERE ID = @ID;";

        try
        {
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@ID", user.ID);
            cmd.Parameters.AddWithValue("@LastLogin", DateTime.Now);

            if (cmd.ExecuteNonQuery() > 0)
                Console.WriteLine($"更新上次登录时间成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"更新上次登录时间失败: {ex.Message}");
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
            WHERE ID = @ID;";

        try
        {
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@ID", user.ID);
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
        // 长度必须在4到10个字符之间
        // "abc"（太短）, "thisusernameistoolong"（太长）, "user@name"（包含特殊字符）
        return Regex.IsMatch(name, @"^[\p{L}\p{N}]{4,10}$");
    }

    private static bool ValidatePassword(string password)
    {
        //必须包含至少一个字母（大小写不限）必须包含至少一个数字 长度至少8个字符 允许包含特殊字符（如!@#$%^&*等）
        // Password123 "password"（缺少数字）, "12345678"（缺少字母）, "abc123"（太短）
        return Regex.IsMatch(password, @"^(?=.*[A-Za-z])(?=.*\d).{8,}$");
    }

    #endregion 验证工具

    #region 执行工具

    private static void ExecuteNonQuery(string sql)
    {
        try
        {
            using var cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"执行失败: {ex.Message}");
        }
    }

    #endregion 执行工具
}