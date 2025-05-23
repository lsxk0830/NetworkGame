﻿/// <summary>
/// 添加玩家、删除玩家、生成MsgGetRoomInfo协议
/// </summary>
public class Room
{
    /// <summary>
    /// 房间ID
    /// </summary>
    public int RoomID = 0;

    /// <summary>
    /// 最大玩家数
    /// </summary>
    public int maxPlayer = 6;

    /// <summary>
    /// 玩家列表
    /// </summary>
    public Dictionary<long, bool> playerIds = new Dictionary<long, bool>();

    /// <summary>
    /// 房主id
    /// </summary>
    public long ownerId = -1;

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
    /// 阵营出生点
    /// </summary>
    private static float[,,] birthConfig = new float[2, 3, 6]
    {
            { { -159f,-26.8f,41.65f,10.272f,173.876f,-3.192f}, { 1,1,1,1,1,1}, { 1,1,1,1,1,1} }, // 阵营1出生点
            { { -73.04f,-28.45f,-71.28f,2,-40,2}, { 2,2,2,2,2,2},{ 2,2,2,2,2,2}} // 阵营2出生点
    };

    /// <summary>
    /// 添加玩家,true-加入成功，false-加入失败
    /// </summary>
    public bool AddPlayer(long id)
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
        player.roomId = this.RoomID;
        // 设置房主
        if (ownerId == -1)
            ownerId = player.ID;
        // 广播
        //Broadcast(ToMsg());
        return true;
    }

    /// <summary>
    /// 删除玩家
    /// </summary>
    public bool RemovePlayer(long id)
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
        if (status == Status.FIGHT) // 战斗状态退出，战斗状态退出游戏视为输掉游戏
        {
            User? user = UserManager.GetUser(player.ID);
            if (user == null) return false;
            user.Lost++;
            MsgLeaveBattle msg = new MsgLeaveBattle();
            msg.id = player.ID;
            Broadcast(msg);
        }
        if (playerIds.Count == 0) // 房间为空
        {
            RoomManager.RemoveRoom(this.RoomID);
            //PlayerManager.Broadcast(RoomManager.ToMsg()); // 告诉全员有房间被删除
        }
        // 广播
        //Broadcast(ToMsg());
        return true;
    }

    /// <summary>
    /// 分配阵营
    /// </summary>
    private int SwitchCamp()
    {
        int count1 = 0;
        int count2 = 0;
        foreach (long id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if (player.camp == 1) count1++;
            if (player.camp == 2) count2++;
        }
        return count1 <= count2 ? 1 : 2;
    }

    /// <summary>
    /// 是否是房主
    /// </summary>
    public bool isOwner(Player player)
    {
        return player.ID == ownerId;
    }

    /// <summary>
    /// 选择房主
    /// </summary>
    private long SwitchOwner()
    {
        foreach (long id in playerIds.Keys) // 选择第一个玩家
        {
            return id;
        }
        return -1; // 房间没人
    }

    /// <summary>
    /// 广播消息
    /// </summary>
    public void Broadcast(MsgBase msg)
    {
        foreach (long id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            player.Send(msg);
        }
    }

    ///// <summary>
    ///// 生成MsgGetRoomInfo协议
    ///// </summary>
    ///// <returns></returns>
    //public MsgBase ToMsg()
    //{
    //    MsgGetRoomInfo msg = new MsgGetRoomInfo();
    //    int count = playerIds.Count;
    //    msg.Players = new PlayerInfo[count];
    //    // Players
    //    int i = 0;
    //    foreach (string RoomID in playerIds.Keys)
    //    {
    //        Player player = PlayerManager.GetPlayer(RoomID);
    //        PlayerInfo playerInfo = new PlayerInfo();
    //        // 赋值
    //        playerInfo.RoomID = player.ID;
    //        playerInfo.camp = player.camp;
    //        playerInfo.win = player.data.Win;
    //        playerInfo.lost = player.data.Lost;
    //        playerInfo.isOwner = isOwner(player) ? 1 : 0;
    //        msg.Players[i] = playerInfo;
    //        i++;
    //    }
    //    return msg;
    //}

    /// <summary>
    /// 能否开战
    /// </summary>
    public bool CanStartBattle()
    {
        if (status != Status.PREPARE) return false; // 已经是战斗状态
        // 统计每个阵营的玩家数
        int count1 = 0;
        int count2 = 0;
        foreach (long id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if (player.camp == 1) count1++;
            else count2++;
        }
        // 每个阵营至少要有 1 名玩家
        if (count1 < 1 || count2 < 1)
            return false;
        return true;
    }

    /// <summary>
    /// 初始化位置
    /// </summary>
    private void SetBirthPos(Player player, int index)
    {
        int camp = player.camp;
        player.x = birthConfig[camp - 1, index, 0];
        player.y = birthConfig[camp - 1, index, 1];
        player.z = birthConfig[camp - 1, index, 2];
        player.ex = birthConfig[camp - 1, index, 3];
        player.ey = birthConfig[camp - 1, index, 4];
        player.ez = birthConfig[camp - 1, index, 5];
    }

    /// <summary>
    /// 重置玩家战斗属性
    /// </summary>
    private void ResetPlayers()
    {
        // 位置和旋转
        int count1 = 0;
        int count2 = 0;
        foreach (long id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            player.hp = 100;
            if (player.camp == 1)
            {
                SetBirthPos(player, count1);
                count1++;
            }
            else
            {
                SetBirthPos(player, count2);
                count2++;
            }
            player.hp = 100;
        }
    }

    /// <summary>
    /// 玩家数据转成TankInfo
    /// </summary>
    public TankInfo PlayerToTankInfo(Player player)
    {
        TankInfo tankInfo = new TankInfo();
        tankInfo.camp = player.camp;
        tankInfo.id = player.ID;
        tankInfo.hp = player.hp;
        tankInfo.x = player.x;
        tankInfo.y = player.y;
        tankInfo.z = player.z;
        tankInfo.ex = player.ex;
        tankInfo.ey = player.ey;
        tankInfo.ez = player.ez;
        return tankInfo;
    }

    /// <summary>
    /// 开战
    /// </summary>
    public bool StartBattle()
    {
        if (!CanStartBattle())
            return false;
        status = Status.FIGHT; // 状态
        ResetPlayers(); // 重置属性
        MsgEnterBattle msg = new MsgEnterBattle();
        msg.mapId = 1;
        msg.tanks = new TankInfo[playerIds.Count];
        int i = 0;
        foreach (long id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            msg.tanks[i] = PlayerToTankInfo(player);
            i++;
        }
        Broadcast(msg);
        return true;
    }

    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool IsDie(Player player)
    {
        return player.hp <= 0;
    }

    /// <summary>
    /// 胜负判断，0-未分出胜负，1-阵营1胜利，2-阵营2胜利
    /// </summary>
    public int Judgment()
    {
        // 存活人数
        int count1 = 0;
        int count2 = 0;
        foreach (long id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            if (!IsDie(player))
            {
                if (player.camp == 1) count1++;
                if (player.camp == 2) count2++;
            }
        }
        // 判断
        if (count1 <= 0)
            return 2;
        else if (count2 <= 0)
            return 1;
        return 0;
    }

    /// <summary>
    /// 上一次判断结果的时间
    /// </summary>

    private long lastJudgeTime = 0;

    /// <summary>
    /// 定时更新,10s更新一次
    /// </summary>
    public void Update()
    {
        if (status != Status.FIGHT) // 状态判断
            return;
        if (NetManager.GetTimeStamp() - lastJudgeTime > 10f) // 时间判断
            return;
        lastJudgeTime = NetManager.GetTimeStamp();
        int winCamp = Judgment(); // 胜负判断
        if (winCamp == 0)
            return;
        status = Status.PREPARE; // 某一方胜利，结束战斗
        foreach (long id in playerIds.Keys)  // 统计信息
        {
            Player player = PlayerManager.GetPlayer(id);
            User user = UserManager.GetUser(id);
            if (player == null || user == null) return;
            if (player.camp == winCamp)
                user.Win++;
            else
                user.Lost++;
        }
        // 发送Result
        MsgBattleResult msg = new MsgBattleResult();
        msg.winCamp = winCamp;
        Broadcast(msg);
    }
}