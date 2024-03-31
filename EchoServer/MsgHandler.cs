using System;

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
        }

        public static void MsgList(ClientState c, string msgArgs)
        {
            Console.WriteLine($"MsgList:{msgArgs}");
        }
    }
}