using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EchoServer
{
    internal class EhoServer
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Socket
            Socket listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1"); // 127.0.0.1是回送地址，指本地机，一般用于测试
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            listenfd.Bind(ipEp); // 给listenfd套接字绑定IP和端口

            // Listen
            listenfd.Listen(0); // backlog指定队列中最多可容纳等待接受的连接数，0表示不限制
            Console.WriteLine("[服务器]启动成功");
            while (true)
            {
                // Accept
                Socket connfd = listenfd.Accept(); // 接收客户端连接
                Console.WriteLine("[服务器]Accept");

                // Receive
                byte[] readBuff = new byte[1024];
                int count = connfd.Receive(readBuff);
                string readStr = Encoding.Default.GetString(readBuff, 0, count);
                Console.WriteLine("[服务器接收]" + readStr);

                // Send
                byte[] sendBytes = Encoding.Default.GetBytes(readStr);
                connfd.Send(sendBytes);
            }
        }
    }
}