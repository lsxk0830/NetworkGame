/// <summary>
/// 同步协议
/// </summary>
public class MsgFrameSync : MsgBase
{
    public MsgFrameSync()
    {
        protoName = "MsgFrameSync";
    }

    /// <summary>
    /// 指令，0-前进 1-后退 2-左转 3-右转 4-停止
    /// </summary>
    public int cmd = 0;

    /// <summary>
    /// 在第几帧发生事件
    /// </summary>
    public int frame = 0;
}