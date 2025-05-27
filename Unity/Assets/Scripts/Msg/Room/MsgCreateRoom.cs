/// <summary>
/// 创建房间
/// </summary>
public class MsgCreateRoom : MsgBase
{
    public MsgCreateRoom()
    {
        protoName = "MsgCreateRoom";
    }

    /// <summary>
    /// 服务器返回的执行结果 0-创建成功 其他数值-创建失败
    /// </summary>
    public int result = 0;

    /// <summary>
    /// 返回的房间ID
    /// </summary>
    public string roomID = "";

    /// <summary>
    /// 指定的用户ID创建的房间
    /// </summary>
    public long ID;
}