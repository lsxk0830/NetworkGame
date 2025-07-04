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

    /// <summary>
    /// 请求退出房间的房间序号（id）
    /// </summary>
    public string roomID;

    /// <summary>
    /// 请求退出的玩家ID
    /// </summary>
    public long ID;
}