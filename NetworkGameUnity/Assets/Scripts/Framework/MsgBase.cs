/*
0 16 0 7 M s g M o v e { " x " = 1 }
0 16 : 【消息长度】示例中“07MsgMove{"x"=1}”的长度，为16字节
0 7: 【协议名长度】示例中“MsgMove”的长度，为7字节。通过协议名长度可以正确解析协议名称，根据名称做消息分发
M s g M o v e：【协议名】长度由“协议名长度”确定
{ " x " = 1 } : 【协议体】可由它解析出MsgMove对象

*/
public class MsgBase
{
    /// <summary>
    /// 协议名
    /// </summary>
    public string protoName = "";
}
