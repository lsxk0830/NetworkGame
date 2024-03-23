using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Echo : MonoBehaviour
{
    private Socket sockect;

    public InputField InputField;

    public Text text;

    // 接收缓冲区
    private byte[] readBuff = new byte[1024];

    private string recvStr = "";

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
        sockect = new Socket(AddressFamily.InterNetwork, // 地址族
                             SocketType.Stream, // 套接字类型
                             ProtocolType.Tcp); // 协议
        // Connect
        //sockect.Connect("127.0.0.1", 8888); // (远程IP地址，远程端口) 阻塞方法会卡住，直到服务器回应
        sockect.BeginConnect("127.0.0.1", 8888, ConnectCallback, sockect);
    }

    /// <summary>
    /// 点击发送按钮
    /// </summary>
    public void Send()
    {
        // Send
        string sendStr = InputField.text;
        byte[] sendBytes = Encoding.Default.GetBytes(sendStr);
        sockect.Send(sendBytes); // 阻塞方法 接受一个byte[]类型的参数指明要发送的内容
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
    /// Receive回调
    /// </summary>
    /// <param name="ar"></param>
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            recvStr = Encoding.Default.GetString(readBuff, 0, count);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket Receive Fail {ex.ToString()}");
        }
    }
}