namespace Tank
{
    /// <summary>
    /// 进入房间
    /// </summary>
    public class MsgEnterRoom : MsgBase
    {
        public MsgEnterRoom()
        {
            protoName = "MsgEnterRoom";
        }

        /// <summary>
        /// 请求加入房间的房间序号（id）
        /// </summary>
        public int id = 0;

        /// <summary>
        /// 服务器返回的执行结果 0-成功进入 其他数值-进入失败
        /// </summary>
        public int result = 0;
    }
}