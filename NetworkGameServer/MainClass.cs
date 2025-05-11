namespace Game
{
    internal class MainClass
    {
        private static async void Main(string[] args)
        {
#if DEBUG
            if (!DbManager.Connect("tankdb", "127.0.0.1", 3306, "root", ""))
#else
            if (!DbManager.Connect("tankdb", "127.0.0.1", 3306, "game_online", "LSXK0830wyyx"))
#endif
            {
                Console.ReadLine();
                return;
            }
            NetManager.StartLoop(8888);

            var manager = new HTTPManager();
            await manager.StartAsync();
            Console.WriteLine("Server started at http://localhost:9988");
        }
    }
}