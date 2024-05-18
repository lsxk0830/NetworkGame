using System;

namespace Game
{
    internal class MainClass
    {
        private static void Main(string[] args)
        {
            if (!DbManager.Connect("game", "127.0.0.1", 3306, "root", ""))
            {
                Console.ReadLine();
                return;
            }
            if (DbManager.Register("lpy", "123456"))
                Console.WriteLine("注册成功");
            NetManager.StartLoop(8888);
        }
    }
}