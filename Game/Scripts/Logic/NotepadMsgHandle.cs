using System;

public partial class MsgHandler
{
    /// <summary>
    /// 获取记事本内容协议处理
    /// </summary>
    public static void MsgGetText(ClientState c, MsgBase msgBase)
    {
        MsgGetText msg = (MsgGetText)msgBase;
        Player player = c.player;
        if (player == null) return;
        //获取text
        msg.text = player.data.text;
        player.Send(msg);
    }

    /// <summary>
    /// 保存记事本内容协议处理
    /// </summary>
    public static void MsgSaveText(ClientState c, MsgBase msgBase)
    {
        MsgSaveText msg = (MsgSaveText)msgBase;
        Player player = c.player;
        if (player == null) return;
        //获取text
        player.data.text = msg.text;
        player.Send(msg);
    }
}