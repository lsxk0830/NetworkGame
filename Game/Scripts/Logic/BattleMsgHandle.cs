using System;

public partial class MsgHandler
{
    /// <summary>
    /// 玩家移动协议处理
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgMove(ClientState c, MsgBase msgBase)
    {
        MsgMove msgMove = (MsgMove)msgBase;
        Console.WriteLine("MsgMove:" + msgMove.x);
        msgMove.x++;
        NetManager.Send(c, msgMove);
    }
}