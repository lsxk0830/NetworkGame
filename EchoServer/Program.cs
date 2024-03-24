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
            Console.WriteLine("Hello World!");

            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1"); // 127.0.0.1是回送地址，指本地机，一般用于测试
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            listenfd.Bind(ipEp); // 给listenfd套接字绑定IP和端口

            // Listen
            listenfd.Listen(0); // backlog指定队列中最多可容纳等待接受的连接数，0表示不限制
            Console.WriteLine("[服务器]启动成功");

            listenfd.BeginAccept(AcceptCallback, listenfd);

            Console.ReadLine();
        }

        /// <summary>
        /// Accept应答回调
        /// </summary>
        private static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("[服务器应答]Accept");
                Socket listenfd = (Socket)ar.AsyncState;
                Socket clientfd = listenfd.EndAccept(ar);

                // clients 列表
                ClientState state = new ClientState();
                state.socket = clientfd;
                clients.Add(clientfd, state);

                // 接收数据BeginReceive
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

                // 继续Accept
                listenfd.BeginAccept(AcceptCallback, listenfd);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket Accept fail {ex.ToString()}");
            }
        }

        /// <summary>
        /// Receive接收回调
        /// </summary>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                ClientState state = (ClientState)ar.AsyncState;
                Socket clientfd = state.socket;
                int count = clientfd.EndReceive(ar);

                // 客户端关闭
                if (count == 0)
                {
                    clientfd.Close();
                    clients.Remove(clientfd);
                    Console.WriteLine($"Socket Close");
                    return;
                }

                string receiveStr = Encoding.Default.GetString(state.readBuff, 0, count);
                string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + receiveStr;
                Console.WriteLine("[服务器接收]" + receiveStr);
                byte[] sendBytes = Encoding.Default.GetBytes(sendStr);

                foreach (ClientState s in clients.Values) // 给所有客户端发送消息
                {
                    s.socket.Send(sendBytes);
                }

                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket Receive fail : {ex.ToString()}");
            }
        }
    }
}