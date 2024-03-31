using System;
using System.Text;

namespace EchoServer
{
    /// <summary>
    /// 消息处理模块
    /// 存放所有信息处理函数
    /// </summary>
    public static class MsgHandler
    {
        /// <summary>
        /// 消息进入
        /// </summary>
        /// <param name="c">哪个客户端发来的</param>
        /// <param name="msgArgs">消息</param>
        public static void MsgEnter(ClientState c, string msgArgs)
        {
            Console.WriteLine($"MsgEnter:{msgArgs}");

            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float eulY = float.Parse(split[4]);

            // 赋值
            c.hp = 100;
            c.x = x;
            c.y = y;
            c.z = z;
            c.eulY = eulY;

            // 广播
            string sendStr = "Enter|" + msgArgs;
            foreach (ClientState cs in MainClass.clients.Values)
            {
                MainClass.Send(cs, sendStr);
            }
        }

        /// <summary>
        /// 组装List协议并发送
        /// </summary>
        /// <param name="c"></param>
        /// <param name="msgArgs"></param>
        public static void MsgList(ClientState c, string msgArgs)
        {
            string sendStr = "List|";
            foreach (ClientState cs in MainClass.clients.Values)
            {
                sendStr += cs.socket.RemoteEndPoint.ToString() + ",";
                sendStr += cs.x.ToString() + ",";
                sendStr += cs.y.ToString() + ",";
                sendStr += cs.z.ToString() + ",";
                sendStr += cs.eulY.ToString() + ",";
                sendStr += cs.hp.ToString() + ",";
            }
            MainClass.Send(c, sendStr);
        }
    }
}