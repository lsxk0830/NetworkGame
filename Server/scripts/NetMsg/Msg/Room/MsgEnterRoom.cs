/// <summary>
/// 进入房间协议
/// </summary>
[Serializable]
public class MsgEnterRoom : MsgBase
{
    public MsgEnterRoom()
    {
        protoName = "MsgEnterRoom";
    }

    /// <summary>
    /// 服务器返回的请求加入房间的房间信息
    /// </summary>
    public Room room { get; set; }

    /// <summary>
    /// 服务器返回的执行结果 0-成功进入 其他数值-进入失败
    /// </summary>
    public int result { get; set; }

    /// <summary>
    /// 请求加入房间的房间序号（id）
    /// </summary>
    public string roomID { get; set; }
}