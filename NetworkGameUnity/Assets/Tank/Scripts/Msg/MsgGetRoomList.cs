namespace Tank
{
    /// <summary>
    /// 获取房间列表
    /// </summary>
    public class MsgGetRoomList : MsgBase
    {
        public MsgGetRoomList()
        {
            protoName = "MsgGetRoomList";
        }

        /// <summary>
        /// 服务器返回的所有房间信息
        /// </summary>
        public RoomInfo[] rooms;
    }
}