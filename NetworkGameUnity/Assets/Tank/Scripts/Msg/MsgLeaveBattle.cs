namespace Tank
{
    /// <summary>
    /// 玩家退出（服务器推送）
    /// </summary>
    public class MsgLeaveBattle : MsgBase
    {
        public MsgLeaveBattle()
        {
            protoName = "MsgLeaveBattle";
        }

        /// <summary>
        /// 服务器返回的玩家Id
        /// </summary>
        public string id = "";
    }
}