using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Tank;

public class Room
{
    /// <summary>
    /// id
    /// </summary>
    public int id = 0;

    /// <summary>
    /// 最大玩家数
    /// </summary>
    public int maxPlayer = 6;

    /// <summary>
    /// 玩家列表
    /// </summary>
    public Dictionary<string, bool> playerIds = new Dictionary<string, bool>();

    /// <summary>
    /// 房主id
    /// </summary>
    public string ownerId = "";

    public Status status = Status.PREPARE;

    /// <summary>
    /// 状态
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// 准备中
        /// </summary>
        PREPARE = 0,

        /// <summary>
        /// 战斗中
        /// </summary>
        FIGHT = 1
    }

    /// <summary>
    /// 添加玩家,true-加入成功，false-加入失败
    /// </summary>
    public bool AddPlayer(string id)
    {
        // 获取玩家
        Player player = PlayerManager.GetPlayer(id);
        if (player == null)
        {
            Console.WriteLine("Room.AddPlayer fail,reach is null");
            return false;
        }
        // 房间人数
        if (playerIds.Count >= maxPlayer)
        {
            Console.WriteLine("Room.AddPlayer fail,reach maxPLayer");
            return false;
        }
        // 准备状态才能加入
        if (status != Status.PREPARE)
        {
            Console.WriteLine("Room.AddPlayer fail,not PREPARE");
            return false;
        }
        // 已经在房间里
        if (playerIds.ContainsKey(id))
        {
            Console.WriteLine("Room.AddPlayer fail,already in this room");
            return false;
        }
        // 加入列表
        playerIds[id] = true;
        // 设置玩家数据
        player.camp = SwitchCamp();
        player.roomId = this.id;
        // 设置房主
        if (ownerId == "")
            ownerId = player.id;
        // 广播
        Broadcast(ToMag());
        return true;
    }

    /// <summary>
    /// 删除玩家
    /// </summary>
    public bool RemovePlayer(string id)
    {
        // 获取玩家
        Player player = PlayerManager.GetPlayer(id);
        if (player == null)
        {
            Console.WriteLine("Room.RemovePlayer fail,player is null");
            return false;
        }
        // 没有在房间里
        if (!playerIds.ContainsKey(id))
        {
            Console.WriteLine("Room.RemovePlayer fail,not in this room");
            return false;
        }
        // 删除列表
        playerIds.Remove(id);
        player.camp = 0;
        player.roomId = -1;
        // 设置房主
        if (isOwner(player))
            ownerId = SwitchOwner();
        // 房间为空
        if (playerIds.Count == 0)
            RoomManager.RemoveRoom(this.id);
        // 广播
        Broadcast(ToMsg());
        return true;
    }

    /// <summary>
    /// 分配阵营
    /// </summary>
    private int SwitchCamp()
    {
        int count1 = 0;
        int count2 = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if (player.camp == 1) count1++;
            if (player.camp == 2) count2++;
        }
        if (count1 <= count2)
            return 1;
        else
            return 2;
    }

    /// <summary>
    /// 是否是房主
    /// </summary>
    private bool isOwner(Player player)
    {
        return player.id == ownerId;
    }

    /// <summary>
    /// 选择房主
    /// </summary>
    private string SwitchOwner()
    {
        foreach (string id in playerIds.Keys) // 选择第一个玩家
        {
            return id;
        }
        return ""; // 房间没人
    }

    /// <summary>
    /// 广播消息
    /// </summary>
    private void Broadcast(MsgBase msg)
    {
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            player.Send(msg);
        }
    }

    /// <summary>
    /// 生成MsgGetRoomInfo协议
    /// </summary>
    /// <returns></returns>
    private MsgBase ToMsg()
    {
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        int count = playerIds.Count;
        msg.players = new PlayerInfo[count];
        // Players
        int i = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            PlayerInfo playerInfo = new PlayerInfo();
            // 赋值
            playerInfo.id = player.id;
            playerInfo.camp = player.camp;
            playerInfo.win = player.data.win;
            playerInfo.lost = player.data.lost;
            playerInfo.isOwner = isOwner(player) ? 1 : 0;
            msg.players[i] = playerInfo;
            i++;
        }
        return msg;
    }
}