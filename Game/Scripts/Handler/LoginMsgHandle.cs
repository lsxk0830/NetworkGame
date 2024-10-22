using System;

public partial class MsgHandler
{
    /// <summary>
    /// 注册协议处理
    /// </summary>
    public static void MsgRegister(ClientState c, MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        if (DbManager.Register(msg.id, msg.pw))
        {
            DbManager.CreatePlayer(msg.id);
            msg.result = 0;
        }
        else
            msg.result = 1;
        NetManager.Send(c, msg);
    }

    /// <summary>
    /// 登陆协议处理
    /// </summary>
    public static void MsgLogin(ClientState c, MsgBase msgBase)
    {
        MsgLogin msg = (MsgLogin)msgBase;
        if (!DbManager.CheckPassword(msg.id, msg.pw)) //密码校验
        {
            Console.WriteLine("账号密码错误");
            msg.result = 1;
            NetManager.Send(c, msg);
            return;
        }
        else
            Console.WriteLine("账号密码正确");
        if (c.player != null)  //不允许再次登陆
        {
            msg.result = 1;
            NetManager.Send(c, msg);
            return;
        }
        if (PlayerManager.IsOnline(msg.id)) //如果已经登陆，踢下线
        {
            //发送踢下线协议
            Player other = PlayerManager.GetPlayer(msg.id);
            MsgKick msgKick = new MsgKick();
            msgKick.reason = 0;
            other.Send(msgKick);
            NetManager.Close(other.state); //断开连接
        }
        PlayerData playerData = DbManager.GetPlayerData(msg.id);    //获取玩家数据
        if (playerData == null)
        {
            msg.result = 1;
            NetManager.Send(c, msg);
            return;
        }
        //构建Player
        Player player = new Player(c);
        player.id = msg.id;
        player.data = playerData;
        PlayerManager.AddPlayer(msg.id, player);
        c.player = player;
        //返回协议
        msg.result = 0;
        player.Send(msg);
    }
}