/// <summary>
/// 战斗结果（服务器推送）
/// </summary>
public class MsgEndBattle : MsgBase
{
    public MsgEndBattle()
    {
        protoName = "MsgEndBattle";
    }

    /// <summary>
    /// 获胜的阵营
    /// </summary>
    public int winCamp = 0;
}