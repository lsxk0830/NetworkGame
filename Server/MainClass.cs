namespace Game
{
    internal class MainClass
    {
        private static void Main(string[] args)
        {
#if DEBUG
            DbManager.Connect("tankdb", "127.0.0.1", 3306, "root", "");
#else
            DbManager.Connect("tankdb", "127.0.0.1", 3306, "game_online", "LSXK0830wyyx");
#endif
            
            //if (DbManager.Register("lpy", "123456"))
            //    Console.WriteLine("注册成功");
            //if (DbManager.CreatePlayer("Sky"))
            //    Console.WriteLine("创建成功");
            //PlayerData pd = DbManager.GetPlayerData("Sky");
            //pd.coin = 256;
            //DbManager.UpdatePlayerData("Sky", pd);



            // 初始化连接
            //DbManager.Connect("127.0.0.1", "game_db", "root", "123456");

            //// 注册新用户
            //long userId = DbManager.Register("玩家小龙", "Pass123!");

            //// 用户登录
            //var user = DbManager.Login("玩家小龙", "Pass123!");
            //if (user != null)
            //{
            //    // 更新数据
            //    user.Win++;
            //    user.AddCoins(50);
            //    DbManager.UpdateUser(user);

            //    // 查看胜率
            //    Console.WriteLine($"胜率: {user.WinRate:F2}%");
            //}
            NetManager.StartLoop(8888);
        }
    }
}