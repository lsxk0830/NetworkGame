using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Echo : MonoBehaviour
{
    private Socket socket; // 定义套接字
    public InputField InputField;
    public Text text;
    private ByteArray readBuff = new ByteArray(); // 接收缓冲区
    private string recvStr = ""; // 显示文字

    private byte[] sendBytes = new byte[1024]; // 发送缓冲区
    private Queue<ByteArray> writeQueue = new Queue<ByteArray>(); // 写入队列

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
        socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
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
        sendBytes = lenBytes.Concat(bodyBytes).ToArray(); // 要发送的数据

        ByteArray ba = new ByteArray(sendBytes);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }

        if (count == 1)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }
    }

    /// <summary>
    /// Connect回调
    /// </summary>
    /// <param name="ar">.Net提供的一种异步操作</param>
    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState; // 由于BeginConnect最后一个参数出入的socket,可由 ar.AsyncState 获取到
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Success");
            // readBuff:接收缓冲区;0:从readBuff第0位开始接收数据;1024:每次最多接收1024个字节的数据
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
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
            int count = socket.EndReceive(ar); // 获取接收数据的长度
            readBuff.writeIdx += count;
            OnReceiveData(); // 处理二进制数据
            //Thread.Sleep(1000 * 30); // 等待
            // 继续接收数据
            if (readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket接收失败: {ex.ToString()}");
        }
    }

    /// <summary>
    /// 数据处理
    /// </summary>
    private void OnReceiveData()
    {
        Debug.Log($"[Recv 1] length = {readBuff.length}");
        Debug.Log($"[Recv 1] readBuff = {readBuff.ToString()}");
        if (readBuff.length <= 2) return;

        // 消息长度
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)(bytes[readIdx + 1] << 8 | bytes[readIdx]);
        if (readBuff.length < bodyLength + 2) return;
        readBuff.readIdx += 2;
        Debug.Log($"[Recv 3] bodyLength = {bodyLength}");

        // 消息体
        byte[] stringByte = new byte[bodyLength];
        readBuff.Read(stringByte, 0, bodyLength);
        string s = Encoding.UTF8.GetString(stringByte);
        Debug.Log($"[Recv 4] s = {s}");
        Debug.Log($"[Recv 5] readBuff = {readBuff.ToString()}");

        // 消息处理
        recvStr = s + "\n" + recvStr;

        // 继续读取消息
        if (readBuff.length > 2)
            OnReceiveData();
    }

    /// <summary>
    /// Send回调
    /// </summary>
    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndSend(ar);
            // 判断是否发送完整

            ByteArray ba;
            lock (writeQueue)
            {
                ba = writeQueue.First();
            }
            ba.readIdx += count;

            if (ba.length == 0) // 发送完整
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();
                    ba = writeQueue.First();
                }
            }

            if (ba != null) // 发送不完整，或发送完整且存在第二条数据
                socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket send fail {ex.ToString()}");
        }
    }
}