namespace Game
{
    internal class MainClass
    {
        private static CancellationTokenSource _cts = new();
        private static async Task Main(string[] args)
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
            //long userId = DbManager.Register("Test1", "QQqqq");

            //// 用户登录
            //User user = DbManager.Login("user123", "Pass123!");
            //if (user != null)
            //{
            //    更新数据
            //    user.Win++;
            //    user.Coin++;
            //    DbManager.UpdateUser(user);
            //    Console.WriteLine($"胜率: {(user.Win / (user.Win + user.Lost)) * 100:F2}%");
            //}

            #endregion 测试数据库

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Cleanup();

            var httpTask = HTTPManager.StartAsync(_cts.Token);
            var networkTask = Task.Run(() => NetManager.StartLoop(8888, _cts.Token));

            await Task.WhenAll(httpTask, networkTask); // 等待所有任务完成
        }


        private static void Cleanup()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}