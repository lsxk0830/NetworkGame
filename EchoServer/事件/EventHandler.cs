using System;

namespace EchoServer
{
    /// <summary>
    /// 事件处理模块
    /// </summary>
    public class EventHandler
    {
        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="c">某个客户端</param>
        public static void OnDisconnect(ClientState c)
        {
            Console.WriteLine($"EventHandler执行OnDisconnect");
        }
    }
}