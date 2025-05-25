// 查询战绩、获取房间列表、创建房间、进入房间、获取房间信息、退出房间、开始战斗

/// <summary>
///// 查询战绩协议
///// </summary>
//public class MsgGetAchieve : MsgBase
//{
//    public MsgGetAchieve() { protoName = "MsgGetAchieve"; }

//    /// <summary>
//    /// 总胜利次数
//    /// </summary>
//    public int win { get; set; } = 0;

//    /// <summary>
//    /// 总失败次数
//    /// </summary>
//    public int lost { get; set; } = 0;
//}

/// <summary>
/// 进入房间
/// </summary>
//public class MsgEnterRoom : MsgBase
//{
//    public MsgEnterRoom() { protoName = "MsgEnterRoom"; }

//    /// <summary>
//    /// 请求加入房间的房间序号（ID）
//    /// </summary>
//    public int ID { get; set; } = 0;

//    /// <summary>
//    /// 服务器返回的执行结果 0-成功进入 其他数值-进入失败
//    /// </summary>
//    public int result { get; set; } = 0;
//}

/// <summary>
/// 获取房间信息
/// </summary>
//public class MsgGetRoomInfo : MsgBase
//{
//    public MsgGetRoomInfo() { protoName = "MsgGetRoomInfo"; }

//    /// <summary>
//    /// 房间的玩家信息
//    /// </summary>
//    public PlayerInfo[] Players { get; set; }
//}

/// <summary>
/// 退出房间
/// </summary>
//public class MsgLeaveRoom : MsgBase
//{
//    public MsgLeaveRoom() { protoName = "MsgLeaveRoom"; }

//    /// <summary>
//    /// 服务器返回的结果 0-离开成功 其他数值-离开失败
//    /// </summary>
//    public int result { get; set; } = 0;
//}

/// <summary>
/// 开始战斗
/// </summary>
//public class MsgStartBattle : MsgBase
//{
//    public MsgStartBattle() { protoName = "MsgStartBattle"; }

//    /// <summary>
//    /// 服务器返回的战斗结果 0-成功 其他数值-失败
//    /// </summary>
//    public int result { get; set; } = 0;
//}