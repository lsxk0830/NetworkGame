namespace SimpleFrame
{
    /// <summary>
    /// Service层接口，Service层脚本都必须继承 IService 接口
    /// </summary>
    public interface IService : ICanGetModel, ICanGetService, ICanGetUtility,
                                ICanEvent
    {
        void Init();
    }
}