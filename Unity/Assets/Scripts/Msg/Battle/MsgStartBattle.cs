/// <summary>
/// 开始战斗
/// </summary>
public class MsgStartBattle : MsgBase
{
    public MsgStartBattle()
    {
        protoName = "MsgStartBattle";
    }

    /// <summary>
    /// 服务器返回的是否战斗结果 0-成功 其他数值-失败
    /// </summary>
    public int result = 0;

    /// <summary>
    /// 战斗开始的房间
    /// </summary>
    public string roomID;

    /// <summary>
    /// 地图大小
    /// </summary>
    public int mapSize;

    /// <summary>
    /// 障碍物数量
    /// </summary>
    public int obstacleCount;
}