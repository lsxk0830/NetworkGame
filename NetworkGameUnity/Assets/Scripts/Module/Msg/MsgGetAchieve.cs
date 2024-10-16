namespace Tank
{
    /// <summary>
    /// 查询战绩协议
    /// </summary>
    public class MsgGetAchieve : MsgBase
    {
        public MsgGetAchieve()
        {
            protoName = "MsgGetAchieve";
        }

        #region 服务端返回

        /// <summary>
        /// 总胜利次数
        /// </summary>
        public int win = 0;

        /// <summary>
        /// 总失败次数
        /// </summary>
        public int lost = 0;

        #endregion
    }
}