namespace SimpleFrame
{
    /// <summary>
    /// 命令接口，所有命令都必须继承 ICommand 接口
    /// </summary>
    public interface ICommand : ICanGetModel, ICanGetService, ICanGetUtility,
                                ICanEvent, ICanCommand
    {
        void Execute();
    }
}