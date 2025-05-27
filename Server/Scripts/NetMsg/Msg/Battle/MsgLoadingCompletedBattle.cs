/// <summary>
/// 战斗准备加载完成（客户端推送）
/// </summary>
public class MsgLoadingCompletedBattle : MsgBase
{
    public MsgLoadingCompletedBattle()
    {
        protoName = "MsgLoadingCompletedBattle";
    }

    /// <summary>
    /// 客户端返回的加载结果 0-成功 其他数值-失败
    /// </summary>
    public int result { get; set; } = 0;

    /// <summary>
    /// 准备战斗的房间
    /// </summary>
    public string roomID;
}