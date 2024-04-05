using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Echo : MonoBehaviour
{
    private Socket socket; // 定义套接字

    public InputField InputField;
    public Text text;

    private byte[] readBuff = new byte[1024]; // 接收缓冲区
    private int buffCount = 0; // 接收缓冲区的数据长度

    private string recvStr = ""; // 显示文字

    private List<Socket> checkRead = new List<Socket>();

    private void Update()
    {
        text.text = recvStr;
    }

    /// <summary>
    /// 点击连接按钮
    /// </summary>
    public void Connection()
    {
        // Socket
        socket = new Socket(AddressFamily.InterNetwork, // 地址族
                            SocketType.Stream, // 套接字类型
                            ProtocolType.Tcp); // 协议
        // Connect
        socket.Connect("127.0.0.1", 8888); // (远程IP地址，远程端口) 阻塞方法会卡住，直到服务器回应
        //socket.BeginConnect("127.0.0.1", 8888, ConnectCallback, socket);
        socket.BeginReceive(readBuff, buffCount, 1024 - buffCount, 0, ReceiveCallback, socket);
    }

    /// <summary>
    /// 点击发送按钮
    /// </summary>
    public void Send()
    {
        string sendStr = InputField.text;
        // 组装协议
        byte[] bodyBytes = Encoding.Default.GetBytes(sendStr);
        Int16 len = (Int16)bodyBytes.Length;
        byte[] lenBytes = BitConverter.GetBytes(len);
        // 大小端编码
        if (!BitConverter.IsLittleEndian)
        {
            Debug.Log($"[Send] Reverse lenBytes");
            lenBytes = (byte[])lenBytes.Reverse();
        }
        byte[] sendBytes = lenBytes.Concat(bodyBytes).ToArray();

        socket.Send(sendBytes); // 阻塞方法 接受一个byte[]类型的参数指明要发送的内容
        Debug.Log($"[Send]{BitConverter.ToString(sendBytes)}");
        //socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
    }

    /// <summary>
    /// Connect回调
    /// </summary>
    /// <param name="ar">.Net提供的一种异步操作</param>
    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // 由于BeginConnect最后一个参数出入的socket,可由 ar.AsyncState 获取到
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Success");
            // readBuff:接收缓冲区;0:从readBuff第0位开始接收数据;1024:每次最多接收1024个字节的数据
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket Connect Fail {ex.ToString()}");
        }
    }

    /// <summary>
    /// Receive 接收消息回调
    /// </summary>
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            // 获取接收数据的长度
            int count = socket.EndReceive(ar);
            buffCount += count;
            // 处理二进制数据
            OnReceiveData();
            // 等待
            Thread.Sleep(1000 * 30);
            // 继续接收数据
            socket.BeginReceive(readBuff, buffCount, 1024 - buffCount, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket接收失败: {ex.ToString()}");
        }
    }

    private void OnReceiveData()
    {
        Debug.Log($"[Recv 1] buffCount = {buffCount}");
        Debug.Log($"[Recv 1] readBuff = {BitConverter.ToString(readBuff)}");
        // 消息长度
        if (buffCount <= 2) return;
        //Int16 bodyLength = BitConverter.ToInt16(readBuff, 0); // 最前面的两个，消息长度
        Int16 bodyLength = (short)(readBuff[1] << 8 | readBuff[0]);
        Debug.Log($"[Recv 3] bodyLength = {bodyLength}");
        // 消息体
        if (buffCount < 2 + bodyLength) return;
        string s = Encoding.UTF8.GetString(readBuff, 2, bodyLength); // 去除消息长度的两位
        Debug.Log($"[Recv 4] s = {s}");
        // 更新缓冲区
        int start = 2 + bodyLength;
        int count = buffCount - start;
        Array.Copy(readBuff, start, readBuff, 0, count);
        buffCount -= start;
        Debug.Log($"[Recv 5] buffCount = {buffCount}");
        // 消息处理
        recvStr = s + "\n" + recvStr;
        OnReceiveData();
    }

    /// <summary>
    /// Send回调
    /// </summary>
    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            socket = (Socket)ar.AsyncState;
            int count = socket.EndSend(ar);
            Debug.Log($"Socket Send Success : {count}");
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket send fail {ex.ToString()}");
        }
    }
}