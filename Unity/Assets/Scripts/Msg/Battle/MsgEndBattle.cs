/// <summary>
/// 战斗结果（客户端推送）
/// </summary>
public class MsgEndBattle : MsgBase
{
    public MsgEndBattle()
    {
        protoName = "MsgEndBattle";
    }

    /// <summary>
    /// 房间ID
    /// </summary>
    public string roomID;

    /// <summary>
    /// 获胜的阵营
    /// </summary>
    public int winCamp = 0;
}