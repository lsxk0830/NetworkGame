namespace Game
{
    internal class MainClass
    {
        private static void Main(string[] args)
        {
            //if (!DbManager.Connect("networkgame", "127.0.0.1", 3306, "NetworkGame", "CYMpKLdxhLpWaHMh"))
            if (!DbManager.Connect("game", "127.0.0.1", 3306, "root", ""))
            {
                Console.ReadLine();
                return;
            }
            //if (DbManager.Register("lpy", "123456"))
            //    Console.WriteLine("注册成功");
            //if (DbManager.CreatePlayer("Sky"))
            //    Console.WriteLine("创建成功");
            //PlayerData pd = DbManager.GetPlayerData("Sky");
            //pd.coin = 256;
            //DbManager.UpdatePlayerData("Sky", pd);
            NetManager.StartLoop(8888);
        }
    }
}