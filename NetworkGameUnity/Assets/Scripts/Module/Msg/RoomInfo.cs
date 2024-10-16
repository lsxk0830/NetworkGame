using System;

namespace Tank
{
    /// <summary>
    /// 房间信息
    /// </summary>
    [Serializable]
    public class RoomInfo
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        public int id = 0;

        /// <summary>
        /// 人数
        /// </summary>
        public int count = 0;

        /// <summary>
        /// 状态 0-准备中 1-战斗中
        /// </summary>
        public int status = 0;
    }
}