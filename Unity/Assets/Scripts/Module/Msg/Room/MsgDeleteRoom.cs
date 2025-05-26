/// <summary>
/// 删除房间
/// </summary>
public class MsgDeleteRoom : MsgBase
{
    public MsgDeleteRoom()
    {
        protoName = "MsgDeleteRoom";
    }

    /// <summary>
    /// 服务器返回的执行结果 0-创建成功 其他数值-创建失败
    /// </summary>
    public int result { get; set; } = 0;

    /// <summary>
    /// 删除的房间ID
    /// </summary>
    public string roomID { get; set; } = "";
}