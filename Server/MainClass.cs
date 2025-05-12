namespace Game
{
    internal class MainClass
    {
        private static void Main(string[] args)
        {
#if DEBUG
            if (!DbManager.Connect("tankdb", "127.0.0.1", 3306, "root", ""))
#else
            if (!DbManager.Connect("tankdb", "127.0.0.1", 3306, "game_online", "LSXK0830wyyx"))
#endif
            {
                return;
            }

            #region 测试数据库

            // 注册新用户
            //long userId = DbManager.Register("abc1", "Pass123#");

            //// 用户登录
            //var user = DbManager.Login("user123", "Pass123!");
            //if (user != null)
            //{
            //    // 更新数据
            //    user.Win++;
            //    user.Coin++;
            //    DbManager.UpdateUser(user);
            //    Console.WriteLine($"胜率: {(user.Win / (user.Win + user.Lost)) * 100:F2}%");
            //}

            #endregion 测试数据库

            //NetManager.StartLoop(8888);
        }
    }
}