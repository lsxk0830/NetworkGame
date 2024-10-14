namespace Tank
{
    /// <summary>
    /// 击中
    /// </summary>
    public class MsgHit : MsgBase
    {
        public MsgHit()
        {
            protoName = "MsgHit";
        }

        /// <summary>
        /// 击中点.x
        /// </summary>
        public float x = 0;

        /// <summary>
        /// 击中点.y
        /// </summary>
        public float y = 0;

        /// <summary>
        /// 击中点.z
        /// </summary>
        public float z = 0;

        /// <summary>
        /// 服务端补充，击中谁
        /// </summary>
        public string targetId = "";

        /// <summary>
        /// 服务端补充，哪个坦克
        /// </summary>
        public string id = "";

        /// <summary>
        /// 服务端补充，被击中坦克血量
        /// </summary>
        public int hp = 0;

        /// <summary>
        /// 服务端补充，收到的伤害
        /// </summary>
        public int damage = 0;
    }
}