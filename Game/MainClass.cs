using System;
using System.Text;

namespace Game
{
    internal class MainClass
    {
        private static void Main(string[] args)
        {
            MsgMove msgMove = new MsgMove();
            msgMove.x = 100;
            msgMove.y = 680;
            byte[] bytes = MsgBase.Encode(msgMove);
            string s = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(s);
            Console.ReadLine();

            string jsonStr = "{\"protoName\":\"MsgMove\",\"x\":100,\"y\":680,\"z\":0}";
            bytes = Encoding.UTF8.GetBytes(jsonStr);
            MsgMove m = (MsgMove)MsgBase.Decode("MsgMove", bytes, 0, bytes.Length);
            Console.WriteLine(m.x);
            Console.WriteLine(m.y);
            Console.WriteLine(m.z);
            Console.ReadLine();
        }
    }
}