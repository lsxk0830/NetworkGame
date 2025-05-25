/// <summary>
/// 进入房间协议
/// </summary>
public class MsgEnterRoom : MsgBase
{
    public MsgEnterRoom()
    {
        protoName = "MsgEnterRoom";
    }

    /// <summary>
    /// 请求加入房间的房间ID
    /// </summary>
    public string roomID { get; set; } = "";

    /// <summary>
    /// 服务器返回的执行结果 0-成功进入 其他数值-进入失败
    /// </summary>
    public int result { get; set; } = 0;

    /// <summary>
    /// 房主id
    /// </summary>
    public long ownerId = -1;

    /// <summary>
    /// 服务器返回的房间玩家信息
    /// </summary>
    public Player[] players { get; set; }
}