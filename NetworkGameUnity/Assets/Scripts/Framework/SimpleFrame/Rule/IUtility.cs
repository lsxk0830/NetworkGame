namespace SimpleFrame
{
    /// <summary>
    /// Utility层接口，Utility层脚本都必须继承 IUtility 接口
    /// </summary>
    public interface IUtility : ICanGetModel, ICanGetService, ICanGetUtility
    {

    }
}