﻿using System.Net.Sockets;

namespace EchoServer
{
    /// <summary>
    /// 保存客户端信息
    /// </summary>
    internal class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
    }
}