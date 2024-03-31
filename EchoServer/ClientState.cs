using System.Net.Sockets;

namespace EchoServer
{
    /// <summary>
    /// 保存客户端信息
    /// </summary>
    public class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];

        public int hp = -100;
        public float x = 0; // 坐标X
        public float y = 0; // 坐标Y
        public float z = 0; // 坐标Z
        public float eulY = 0; // 旋转值
    }
}