using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EchoServer
{
    internal class EhoServer
    {
        // 监听Socket
        private static Socket listenfd;

        // 客户端Socket及状态信息
        private static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        private static void Main(string[] args)
        {
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1"); // 127.0.0.1是回送地址，指本地机，一般用于测试
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            listenfd.Bind(ipEp); // 给listenfd套接字绑定IP和端口

            // Listen
            listenfd.Listen(0); // backlog指定队列中最多可容纳等待接受的连接数，0表示不限制
            Console.WriteLine("[服务器]启动成功");

            // checkRead
            List<Socket> checkRead = new List<Socket>();

            while (true)
            {
                // 填充checkRead列表
                checkRead.Clear();
                checkRead.Add(listenfd);
                foreach (ClientState s in clients.Values)
                {
                    checkRead.Add(s.socket);
                }

                // Select
                Socket.Select(checkRead, null, null, 1000);

                // 检查可读对象
                foreach (Socket s in checkRead)
                {
                    if (s == listenfd)
                        ReadListenfd(s);
                    else
                        ReadClientfd(s);
                }
            }
        }

        /// <summary>
        /// 读取Listenfd
        /// </summary>
        private static void ReadListenfd(Socket listenfd)
        {
            Console.WriteLine("[服务器应答]Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
        }

        /// <summary>
        /// 读取Clientfd
        /// </summary>
        private static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }
            catch (SocketException ex)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine($"Receive SocketException : {ex.ToString()}");
                return false;
            }

            // 客户端关闭
            if (count == 0)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine($"Socket Close");
                return false;
            }

            // 广播
            string receiveStr = Encoding.Default.GetString(state.readBuff, 0, count);
            Console.WriteLine("[服务器接收]" + receiveStr);
            string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + receiveStr;
            byte[] sendBytes = Encoding.Default.GetBytes(sendStr);
            foreach (ClientState s in clients.Values) // 给所有客户端发送消息
            {
                s.socket.Send(sendBytes);
            }
            return true;
        }
    }
}