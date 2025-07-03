/// <summary>
/// 进入战场（服务器推送）
/// </summary>
public class MsgEnterBattle : MsgBase
{
    public MsgEnterBattle()
    {
        protoName = "MsgEnterBattle";
    }

    /// <summary>
    /// 服务器返回的是否战斗结果 0-成功 其他数值-失败
    /// </summary>
    public int result = 0;

    /// <summary>
    /// 房间ID
    /// </summary>
    public string roomID;

    /// <summary>
    /// 服务器返回的坦克列表信息
    /// </summary>
    public Player[] tanks;
}