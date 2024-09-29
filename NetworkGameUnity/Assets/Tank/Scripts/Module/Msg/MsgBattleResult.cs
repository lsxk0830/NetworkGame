namespace Tank
{
    /// <summary>
    /// 战斗结果（服务器推送）
    /// </summary>
    public class MsgBattleResult : MsgBase
    {
        public MsgBattleResult()
        {
            protoName = "MsgBattleResult";
        }

        /// <summary>
        /// 获胜的阵营
        /// </summary>
        public int winCamp = 0;
    }
}