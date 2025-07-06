/// <summary>
/// 添加玩家、删除玩家、生成MsgGetRoomInfo协议
/// </summary>
public class Room : IDisposable
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
    public List<long> camp1List = new(); // 玩家列表，方便遍历
    public List<long> camp2List = new(); // 玩家列表，方便遍历

    /// <summary>
    /// 房主id
    /// </summary>
    public long ownerId = -1;

    public int status = (int)Status.PREPARE;

    public int loadSuccess = 0; // 加载成功的玩家数
    private int delaySeconds = 3; // 最长等待时间，单位秒

    private Dictionary<int, ObstaclePosRotScale> obs;// 障碍物列表

    public int GetAllObstacleCount = 0;// 获取所有障碍物的次数

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

    #region Set信息

    /// <summary>
    /// 更新障碍物信息
    /// </summary>
    public void SetObstaclePosRotScale(int obID, ObstaclePosRotScale ObstaclePosRotScale)
    {
        if (obs.TryGetValue(obID, out ObstaclePosRotScale value))
        {
            value = ObstaclePosRotScale; // 更新障碍物位置
        }
    }

    #endregion Set信息

    /// <summary>
    /// 阵营出生点
    /// </summary>
    private static float[,,] birthConfig = new float[2, 3, 6]
    {
        { { 4f,1f,5f,0f,0f,0f},  { 4f,1f,15f,0f,0f,0f}, { 4f,1f,40f,0f,0f,0f} },// 阵营1出生点
        { { 45f,1f,40f,0f,0f,0f}, { 45f,1f,30f,0f,0f,0f},{ 45f,1f,20f,0f,0f,0f}} // 阵营2出生点
    };

    #region 添加玩家、删除玩家、获取玩家

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
            NetManager.Send(newPlayer.state, msg); // 发送消息给玩家
            Console.WriteLine("房间添加玩家失败，玩家已在房间中");
            return;
        }
        if (playerIds.Count >= maxPlayer)
        {
            NetManager.Send(newPlayer.state, msg);
            Console.WriteLine("房间添加玩家失败，房间人数已满");
            return;
        }
        if ((Room.Status)status != Status.PREPARE)
        {
            NetManager.Send(newPlayer.state, msg);
            Console.WriteLine("房间添加玩家失败，房间已在战斗中");
            return;
        }
        playerIds.Add(newPlayer.ID, newPlayer);

        // 设置玩家数据
        newPlayer.camp = SwitchCamp();

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

    #endregion 添加玩家、删除玩家、获取玩家

    #region 分配阵营、选择房主

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

    #endregion 分配阵营、选择房主

    #region 广播消息、广播开战、广播进入战斗

    /// <summary>
    /// 广播消息
    /// </summary>
    public void Broadcast(MsgBase msg)
    {
        foreach (Player player in playerIds.Values)
        {
            NetManager.Send(player.state, msg); // 发送消息给每个玩家
        }
    }

    /// <summary>
    /// 广播消息除了发送者
    /// </summary>
    public void BroadcastExceptCS(long id, MsgBase msg)
    {
        foreach (var item in playerIds)
        {
            if (item.Key != id) // 排除发送者
                NetManager.Send(item.Value.state, msg); // 发送消息给每个玩家
        }
    }

    /// <summary>
    /// 广播进入战斗消息
    /// </summary>
    private void BroadcastEnterBattle()
    {
        Console.WriteLine("广播进入战斗消息");
        MsgEnterBattle msg = new MsgEnterBattle()
        {
            result = 0,
            tanks = new Player[playerIds.Count]
        };
        ResetPlayers(); // 重置属性
        int i = 0;
        foreach (Player player in playerIds.Values)
        {
            player.skin = new Random().Next(1, 7); // 随机皮肤ID
            msg.tanks[i] = player;
            i++;
        }
        Broadcast(msg); // 广播消息
    }

    /// <summary>
    /// 获取所有障碍物数据信息
    /// </summary>
    public MsgObstacleAll GetAllObstacle(MsgObstacleAll msg)
    {
        if (obs == null || obs.Count == 0) return null; // 没有障碍物
        if (msg.PosRotScales == null)
            msg.PosRotScales = new List<MsgObstacleOne>();
        else
            msg.PosRotScales.Clear(); // 清空之前的障碍物数据
        foreach (var item in obs)
        {
            msg.PosRotScales.Add(new MsgObstacleOne() { ObstacleID = item.Key, PosRotScale = item.Value });
        }
        return msg;
    }

    #endregion 广播消息、广播开战、广播进入战斗

    /// <summary>
    /// 创建随机障碍物
    /// </summary>
    /// <param name="mapSize">地图的大小</param>
    /// <param name="obstacleCount">随机物数量</param>
    public void CreateRandomObstacle(int mapSize, int obstacleCount)
    {
        // 创建随机障碍物逻辑
        Console.WriteLine("创建随机障碍物");
        Random random = new Random();
        obs = new Dictionary<int, ObstaclePosRotScale>(obstacleCount);
        for (int i = 0; i < obstacleCount; i++)
        {
            ObstaclePosRotScale posRotScale = new ObstaclePosRotScale()
            {
                PosX = random.Next(2, mapSize - 4),
                ScaleY = random.Next(1, 3),
                PosZ = random.Next(2, mapSize - 4),
                RotX = 0,
                RotY = random.Next(0, 360),
                RotZ = 0,
                ScaleX = random.Next(1, 3),
                ScaleZ = random.Next(1, 3)
            };
            posRotScale.PosY = posRotScale.ScaleY / 2;
            obs.Add(i, posRotScale);
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
                Console.WriteLine($"达到最大延时时间");
                BroadcastEnterBattle(); // 否则广播进入战斗消息
                return;
            });
        }
        if (loadSuccess >= playerIds.Count)
        {
            Console.WriteLine($"所有玩家都加载成功");
            BroadcastEnterBattle(); // 如果所有玩家都加载成功，广播进入战斗消息
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
        if (count1 < 1 || count2 < 1) return false; // 每个阵营至少要有 1 名玩家

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
        camp1List.Clear();
        camp2List.Clear();
        foreach (Player player in playerIds.Values)
        {
            player.hp = 100;
            if (player.camp == 1)
            {
                SetBirthPos(player, count1);
                count1++;
                camp1List.Add(player.ID); // 添加到阵营1列表
            }
            else
            {
                SetBirthPos(player, count2);
                count2++;
                camp2List.Add(player.ID); // 添加到阵营2列表
            }
        }
    }

    /// <summary>
    /// 胜负判断，0-未分出胜负，1-阵营1胜利，2-阵营2胜利
    /// </summary>
    public int Judgment(int camp, long id)
    {
        if (camp == 1)
            camp1List.Remove(id); // 从阵营1列表中移除玩家
        else if (camp == 2)
            camp2List.Remove(id); // 从阵营2列表中移除玩家

        if (camp1List.Count <= 0)
            return 2;
        else if (camp2List.Count <= 0)
            return 1;
        else
            return 0;
    }

    public void Dispose()
    {
        RoomID = "";
        playerIds?.Clear();
        playerIds = null;
        camp2List.Clear();
        camp1List.Clear();
        camp1List = null;
        camp2List = null;
        obs?.Clear();
        obs = null;
    }
}