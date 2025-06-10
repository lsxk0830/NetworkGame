/// <summary>
/// 添加玩家、删除玩家、生成MsgGetRoomInfo协议
/// </summary>
public class Room
{
    /// <summary>
    /// 房间ID
    /// </summary>
    public string RoomID = "";

    /// <summary>
    /// 地图ID
    /// </summary>
    public int mapId = -1;

    /// <summary>
    /// 最大玩家数
    /// </summary>
    public int maxPlayer = 6;

    /// <summary>
    /// 玩家列表
    /// </summary>
    public Dictionary<long, Player> playerIds = new();

    /// <summary>
    /// 房主id
    /// </summary>
    public long ownerId = -1;

    public int status = (int)Status.PREPARE;

    public int loadSuccess = 0; // 加载成功的玩家数
    private int delaySeconds = 10; // 最长等待时间，单位秒

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

    #region 对玩家的操作

    /// <summary>
    /// 创建房间时添加玩家
    /// </summary>
    public bool AddPlayer(Player newPlayer)
    {
        if (newPlayer == null)
        {
            Console.WriteLine("房间添加玩家失败，要添加的玩家是空");
            return false;
        }
        if (playerIds.ContainsKey(newPlayer.ID))
        {
            Console.WriteLine("房间添加玩家失败，玩家已在房间中");
            return false;
        }
        if (playerIds.Count >= maxPlayer)
        {
            Console.WriteLine("房间添加玩家失败，房间人数已满");
            return false;
        }
        if ((Room.Status)status != Status.PREPARE)
        {
            Console.WriteLine("房间添加玩家失败，房间已在战斗中");
            return false;
        }
        playerIds.Add(newPlayer.ID, newPlayer);

        // 设置玩家数据
        newPlayer.camp = SwitchCamp();
        newPlayer.roomId = this.RoomID;
        ownerId = newPlayer.ID; // 设置房主
        return true;
    }

    /// <summary>
    /// 进入房间时添加玩家
    /// </summary>
    public void EnterRoomAddPlayer(Player newPlayer)
    {
        MsgEnterRoom msg = new MsgEnterRoom() { result = -1 };
        if (newPlayer == null)
        {
            Console.WriteLine("房间添加玩家失败，要添加的玩家是空");
            return;
        }
        if (playerIds.ContainsKey(newPlayer.ID))
        {
            newPlayer.Send(msg);
            Console.WriteLine("房间添加玩家失败，玩家已在房间中");
            return;
        }
        if (playerIds.Count >= maxPlayer)
        {
            newPlayer.Send(msg);
            Console.WriteLine("房间添加玩家失败，房间人数已满");
            return;
        }
        if ((Room.Status)status != Status.PREPARE)
        {
            newPlayer.Send(msg);
            Console.WriteLine("房间添加玩家失败，房间已在战斗中");
            return;
        }
        playerIds.Add(newPlayer.ID, newPlayer);

        // 设置玩家数据
        newPlayer.camp = SwitchCamp();
        newPlayer.roomId = this.RoomID;

        msg.roomID = this.RoomID;
        msg.result = 0;
        msg.room = this;
        Broadcast(msg); // 广播
    }

    /// <summary>
    /// 删除玩家
    /// </summary>
    public bool RemovePlayer(long id)
    {
        // 获取玩家
        if (!playerIds.ContainsKey(id))
        {
            Console.WriteLine("房间移除玩家失败,房间不存在该玩家");
            return false;
        }
        Player? player = playerIds[id];
        if (player == null)
        {
            Console.WriteLine("房间移除玩家失败，玩家为空");
            return false;
        }
        playerIds.Remove(id); // 移除列表
        if (id == ownerId) ownerId = SwitchOwner();// 设置房主
        if (ownerId == -1 || playerIds.Count == 0)
        {
            UserManager.SendExcept(player.state, new MsgDeleteRoom()
            {
                result = 0,
                roomID = this.RoomID,
            });// 全员通知
            RoomManager.RemoveRoom(this.RoomID);
            return true;
        }
        if ((Room.Status)status == Status.FIGHT) // 战斗状态退出，战斗状态退出游戏视为输掉游戏
        {
            User? user = UserManager.GetUser(id);
            if (user == null) return false;
            user.Lost++;
            DbManager.UpdateUser(user);
            MsgLeaveBattle msg = new MsgLeaveBattle()
            {
                id = player.ID
            };
            Broadcast(msg);
        }
        // 广播
        Broadcast(new MsgLeaveRoom()
        {
            ID = id,
            OwnerID = ownerId
        });
        player = null;
        return true;
    }

    public Player? GetPlayer(long id)
    {
        if (playerIds.ContainsKey(id))
            return playerIds[id];
        return null; // 玩家不存在
    }

    #endregion 对玩家的操作

    #region 内部实现

    /// <summary>
    /// 分配阵营
    /// </summary>
    private int SwitchCamp()
    {
        int count1 = 0;
        int count2 = 0;
        foreach (Player player in playerIds.Values)
        {
            if (player.camp == 1) count1++;
            if (player.camp == 2) count2++;
        }
        return count1 <= count2 ? 1 : 2;
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
    private void Broadcast(MsgBase msg)
    {
        foreach (Player player in playerIds.Values)
        {
            player.Send(msg);
        }
    }

    #endregion 内部实现

    /// <summary>
    /// 广播开战消息
    /// </summary>
    public void StartBattle(ClientState cs, MsgBase msg)
    {
        status = (int)Status.FIGHT; // 状态设置为战斗中
        foreach (Player player in playerIds.Values)
        {
            if (player.state == cs) continue; // 排除发送者
            player.Send(msg);
        }
    }

    /// <summary>
    /// 所有加载完成，准备进入游戏
    /// </summary>
    public void LoadSuccess()
    {
        loadSuccess++;

        if (loadSuccess == 1) // 以收到一个玩家的加载成功消息为准，开始计时
        {
            Console.WriteLine($"当前时间：{DateTime.Now}");
            Task.Delay(delaySeconds * 1000).ContinueWith(_ =>
            {
                Console.WriteLine($"当前时间：{DateTime.Now}");
                if (loadSuccess >= playerIds.Count) return; // 如果加载成功的玩家数已经达到要求，则不再发送消息
                MsgEnterBattle msgEnterBattle = new MsgEnterBattle()
                {
                    tanks = new TankInfo[playerIds.Count]
                };
                foreach (Player player in playerIds.Values)
                {
                    player.Send(msgEnterBattle);
                }
            });
        }
        if (loadSuccess >= playerIds.Count)
        {
            MsgEnterBattle msgEnterBattle = new MsgEnterBattle()
            {
                tanks = new TankInfo[playerIds.Count]
            };
            foreach (Player player in playerIds.Values)
            {
                player.Send(msgEnterBattle);
            }
        }// 已经加载成功
    }

    /// <summary>
    /// 能否开战
    /// </summary>
    public bool CanStartBattle()
    {
        if ((Room.Status)status == Status.FIGHT) return false; // 已经是战斗状态
        // 统计每个阵营的玩家数
        int count1 = 0;
        int count2 = 0;
        foreach (Player player in playerIds.Values)
        {
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
        foreach (Player player in playerIds.Values)
        {
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
        TankInfo tankInfo = new TankInfo()
        {
            camp = player.camp,
            id = player.ID,
            hp = player.hp,
            x = player.x,
            y = player.y,
            z = player.z,
            ex = player.ex,
            ey = player.ey,
            ez = player.ez
        };
        return tankInfo;
    }

    /// <summary>
    /// 开战
    /// </summary>
    public bool StartBattle()
    {
        if (!CanStartBattle()) return false;
        status = (int)Status.FIGHT; // 状态
        ResetPlayers(); // 重置属性
        MsgEnterBattle msg = new MsgEnterBattle()
        {
            tanks = new TankInfo[playerIds.Count]
        };
        int i = 0;
        foreach (Player player in playerIds.Values)
        {
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
        foreach (Player player in playerIds.Values)
        {
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
        if ((Room.Status)status != Status.FIGHT) // 状态判断
            return;
        if (NetManager.GetTimeStamp() - lastJudgeTime > 10f) // 时间判断
            return;
        lastJudgeTime = NetManager.GetTimeStamp();
        int winCamp = Judgment(); // 胜负判断
        if (winCamp == 0)
            return;
        status = (int)Status.PREPARE; // 某一方胜利，结束战斗
        foreach (Player player in playerIds.Values)  // 统计信息
        {
            if (player == null) continue;
            User? user = UserManager.GetUser(player.ID);
            if (user == null) continue;
            if (player.camp == winCamp)
                user.Win++;
            else
                user.Lost++;
        }
        // 发送Result
        MsgEndBattle msg = new MsgEndBattle();
        msg.winCamp = winCamp;
        Broadcast(msg);
    }
}