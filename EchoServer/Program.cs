using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace EchoServer
{
    internal class MainClass
    {
        // 监听Socket
        private static Socket listenfd;

        // 客户端Socket及状态信息
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

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
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei?.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine($"Receive SocketException : {ex.ToString()}");
                return false;
            }

            // 客户端关闭
            if (count == 0)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei?.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine($"Socket Close");
                return false;
            }

            // 广播
            string receiveStr = Encoding.Default.GetString(state.readBuff, 0, count);
            string[] split = receiveStr.Split('|');
            Console.WriteLine("[服务器接收]" + receiveStr);
            string msgName = split[0];
            string msgArgs = split[1];
            string funName = "Msg" + msgName;
            MethodInfo mi = typeof(MsgHandler).GetMethod(funName); // MsgEnter,MsgList,MsgMove
            object[] o = { state, msgArgs }; // 客户端状态，消息内容
            mi?.Invoke(null, o);// 参数1:代表this指针，消息处理都是静态方法，所以填null,参数2：参数列表。P73
            return true;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public static void Send(ClientState cs, string sendStr)
        {
            byte[] sendbytes = Encoding.Default.GetBytes(sendStr);
            cs.socket.Send(sendbytes);
        }
    }
}