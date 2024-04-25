using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Example
{
    public static class NetManager
    {
        private static Socket socket; // 定义套接字
        private static byte[] readBuff = new byte[1024]; // 定义缓冲区

        public delegate void MsgListener(string str); // 委托类型

        private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>(); // 监听列表
        private static List<string> msgList = new List<string>(); // 消息列表

        /// <summary>
        /// 连接
        /// </summary>
        public static void Connect(string ip, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(ip, port);

            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiceCallback, socket);
        }

        /// <summary>
        /// 添加消息监听
        /// </summary>
        public static void AddListener(string msgName, MsgListener listener)
        {
            listeners[msgName] = listener;
        }

        /// <summary>
        /// 发送消息给服务端
        /// </summary>
        public static void Send(string sendStr)
        {
            if (socket == null) return;

            if (!socket.Connected) return;

            byte[] sendBytes = Encoding.Default.GetBytes(sendStr);
            //socket.Send(sendBytes);
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket Send fail" + ex.ToString());
            }
        }

        /// <summary>
        /// 辅助方法，获取描述
        /// </summary>
        public static string GetDesc()
        {
            if (socket == null) return "";

            if (!socket.Connected) return "";

            return socket.LocalEndPoint.ToString();
        }

        /// <summary>
        /// 需要外部调用，以驱动NetManager
        /// </summary>
        public static void Update()
        {
            if (msgList.Count <= 0) return;

            string msgStr = msgList[0];
            msgList.RemoveAt(0);
            string[] split = msgStr.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];
            // 监听回调
            if (listeners.ContainsKey(msgName))
            {
                listeners[msgName]?.Invoke(msgArgs);
            }
        }

        private static void ReceiceCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndReceive(ar);
                string reveStr = Encoding.Default.GetString(readBuff, 0, count);
                msgList.Add(reveStr);
                socket.BeginReceive(readBuff, 0, 1024, 0, ReceiceCallback, socket);
            }
            catch (SocketException ex)
            {
                Debug.LogError($"Socket Receive fail:{ex.ToString()}");
            }
        }
    }
}