/// <summary>
/// 开始战斗
/// </summary>
public class MsgStartBattle : MsgBase
{
    public MsgStartBattle() { protoName = "MsgStartBattle"; }

    /// <summary>
    /// 服务器返回的是否战斗结果 0-成功 其他数值-失败
    /// </summary>
    public int result = 0;
}