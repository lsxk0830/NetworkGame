namespace Tank
{
    /// <summary>
    /// 退出房间
    /// </summary>
    public class MsgLeaveRoom : MsgBase
    {
        public MsgLeaveRoom()
        {
            protoName = "MsgLeaveRoom";
        }

        /// <summary>
        /// 服务器返回的结果 0-离开成功 其他数值-离开失败
        /// </summary>
        public int result = 0;
    }
}