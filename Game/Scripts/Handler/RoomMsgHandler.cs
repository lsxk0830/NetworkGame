/// <summary>
/// 查询战绩、请求房间列表、创建房间、进入房间、获取房间信息、离开房间
/// </summary>
public partial class MsgHandler
{
    /// <summary>
    /// 查询战绩
    /// </summary>
    public static void MsgGetAchieve(ClientState c, MsgBase msgBase)
    {
        MsgGetAchieve msg = (MsgGetAchieve)msgBase;
        Player player = c.player;
        if (player == null)
            return;
        msg.win = player.data.win;
        msg.lost = player.data.lost;
        player.Send(msg);
    }

    /// <summary>
    /// 请求房间列表
    /// </summary>
    public static void MsgGetRoomList(ClientState c, MsgBase msgBase)
    {
        Player player = c.player;
        if (player == null)
            return;
        player.Send(RoomManager.ToMsg());
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    public static void MsgCreateRoom(ClientState c, MsgBase msgBase)
    {
        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        Player player = c.player;
        if (player == null)
            return;
        // 已经在房间里
        if (player.roomId >= 0)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        // 创建
        Room room = RoomManager.AddRoom();
        room.AddPlayer(player.id);
        msg.result = 0;
        player.Send(msg);
        PlayerManager.Broadcast(RoomManager.ToMsg()); // 告诉全员有新房间
    }

    /// <summary>
    /// 进入房间
    /// </summary>
    public static void MsgEnterRoom(ClientState c, MsgBase msgBase)
    {
        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        Player player = c.player;
        if (player == null)
            return;
        // 已经在房间里
        if (player.roomId >= 0)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        // 获取房间
        Room room = RoomManager.GetRoom(msg.id);
        if (room == null)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        // 进入
        if (!room.AddPlayer(player.id))
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        // 返回协议
        msg.result = 0;
        player.Send(msg);
    }

    /// <summary>
    /// 获取房间信息
    /// </summary>
    public static void MsgGetRoomInfo(ClientState c, MsgBase msgBase)
    {
        MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
        Player player = c.player;
        if (player == null)
            return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            player.Send(msg);
            return;
        }
        player.Send(room.ToMsg());
    }

    /// <summary>
    /// 离开房间
    /// </summary>
    public static void MsgLeaveRoom(ClientState c, MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        Player player = c.player;
        if (player == null)
            return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        room.RemovePlayer(player.id);
        // 返回协议
        msg.result = 0;
        player.Send(msg);
    }

    /// <summary>
    /// 请求开始战斗
    /// </summary>
    public static void MsgStartBattle(ClientState c, MsgBase msgBase)
    {
        MsgStartBattle msg = (MsgStartBattle)msgBase;
        Player player = c.player;
        if (player == null)
            return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) // Room是否存在
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        if (!room.isOwner(player)) // 是否是房主
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        if (!room.StartBattle()) // 开战
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        // 成功
        msg.result = 0;
        player.Send(msg);
    }
}