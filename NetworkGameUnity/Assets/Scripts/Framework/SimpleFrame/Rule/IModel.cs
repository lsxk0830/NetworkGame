namespace SimpleFrame
{
    /// <summary>
    /// Model层接口，Model层脚本都必须继承 IModel 接口
    /// </summary>
    public interface IModel : ICanGetModel, ICanGetService, ICanGetUtility,
                              ICanEvent
    {
        void Init();
    }
}