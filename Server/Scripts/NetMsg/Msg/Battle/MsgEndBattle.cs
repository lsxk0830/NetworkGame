/// <summary>
/// 战斗结果（客户端推送）
/// </summary>
public class MsgEndBattle : MsgBase
{
    public MsgEndBattle()
    {
        protoName = "MsgBattleResult";
    }

    public string roomID { get; set; }

    /// <summary>
    /// 获胜的阵营
    /// </summary>
    public int winCamp { get; set; } = 0;
}