/*
0 16 0 7 M s g M o v e { " x " = 1 }
0 16 : 【消息长度】示例中“07MsgMove{"x"=1}”的长度，为16字节
0 7: 【协议名长度】示例中“MsgMove”的长度，为7字节。通过协议名长度可以正确解析协议名称，根据名称做消息分发
M s g M o v e：【协议名】长度由“协议名长度”确定
{ " x " = 1 } : 【协议体】可由它解析出MsgMove对象
*/

using Newtonsoft.Json;
using System.Text;

public class MsgBase
{
    /// <summary>
    /// 协议名
    /// </summary>
    public string protoName = "null";

    /// <summary>
    /// 编码协议体
    /// </summary>
    public static byte[] Encode(MsgBase msgBase)
    {
        string s = JsonConvert.SerializeObject(msgBase);
        return Encoding.UTF8.GetBytes(s);
    }

    /// <summary>
    /// 解码协议体
    /// </summary>
    /// <param name="protoName">协议名</param>
    /// <param name="bytes">要解码的byte数组</param>
    /// <param name="offset">byte数组开始位置</param>
    /// <param name="count">byte数组从开始位置要解析的字节数</param>
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        string s = Encoding.UTF8.GetString(bytes, offset, count);
        MsgBase msgBase = (MsgBase)JsonConvert.DeserializeObject(s, Type.GetType(protoName));
        return msgBase;
    }

    /// <summary>
    /// 编码协议名（2字节长度+字符串）
    /// </summary>
    public static byte[] EncodeName(MsgBase msgBase)
    {
        // 名字bytes和长度
        byte[] nameBytes = Encoding.UTF8.GetBytes(msgBase.protoName);
        Int16 len = (Int16)nameBytes.Length;
        byte[] bytes = new byte[2 + len]; // 申请bytes数组
        bytes[0] = ((byte)(len >> 8)); // len / 256
        bytes[1] = (byte)(len & 0xFF); //len % 256
        Array.Copy(nameBytes, 0, bytes, 2, len); // 组装名字bytes
        return bytes;
    }

    /// <summary>
    /// 解码协议名（2字节长度+字符串）
    /// </summary>
    public static string DecodeName(byte[] bytes, int readIndex, out int count)
    {
        count = 0;
        // 必须大于2字节
        if (readIndex + 2 > bytes.Length) return "";
        // 读取长度
        Int16 len = (Int16)(bytes[readIndex] << 8 | bytes[readIndex + 1]);
        // 长度必须足够
        if (readIndex + 2 + len > bytes.Length) return "";
        // 解析
        count = 2 + len;
        string name = Encoding.UTF8.GetString(bytes, readIndex + 2, len);
        return name;
    }
}