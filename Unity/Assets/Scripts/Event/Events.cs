public static class Events
{
    #region 服务器

    public static string SocketOnConnectSuccess = "SocketOnConnectSuccess"; //Socket连接成功事件
    public static string SocketOnConnectFail = "SocketOnConnectFail"; //Socket连接失败事件

    #endregion 服务器

    #region 消息协议
    public static string MsgPing = "MsgPing"; //心跳协议
    public static string MsgKick = "MsgKick"; //被踢下线

    public static string MsgGetRooms = "MsgGetRooms"; //获取房间列表
    public static string MsgCreateRoom = "MsgCreateRoom"; //新建房间
    public static string MsgDeleteRoom = "MsgDeleteRoom"; //删除房间
    public static string MsgEnterRoom = "MsgEnterRoom"; //进入房间
    public static string MsgLeaveRoom = "MsgLeaveRoom"; //离开房间

    public static string MsgStartBattle = "MsgStartBattle"; //开战
    public static string MsgObstacleAll = "MsgObstacleAll"; //随机生成场景中所有方块(障碍物)消息
    public static string MsgObstacleOne = "MsgObstacleOne"; //随机生成场景中一个方块(障碍物)消息
    public static string MsgEnterBattle = "MsgEnterBattle"; //战斗协议
    public static string MsgBattleResult = "MsgBattleResult"; //战斗结束协议
    public static string MsgLeaveBattle = "MsgLeaveBattle"; //玩家退出协议

    public static string MsgSyncTank = "MsgSyncTank"; //同步协议
    public static string MsgFire = "MsgFire"; //开火协议
    public static string MsgHit = "MsgHit"; //击中协议

    #endregion 消息协议

    #region UI事件
    public static string PanelLoadSuccess = "PanelLoadSuccess"; //面板加载成功事件

    #endregion UI事件
}
