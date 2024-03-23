using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Echo : MonoBehaviour
{
    private Socket sockect;

    public InputField InputField;

    public Text text;

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
        sockect.Connect("127.0.0.1", 8888); // (远程IP地址，远程端口) 阻塞方法会卡住，直到服务器回应
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
        // Recv
        byte[] readBuff = new byte[1024];
        int count = sockect.Receive(readBuff); // 阻塞方法 Receive带有一个byte[]类型的参数,它存储接收到的消息
        string recvStr = Encoding.Default.GetString(readBuff, 0, count);
        text.text = recvStr;
        // Close
        sockect.Close();
    }
}