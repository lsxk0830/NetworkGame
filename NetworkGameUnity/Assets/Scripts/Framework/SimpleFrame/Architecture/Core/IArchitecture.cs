namespace SimpleFrame
{
    public interface IArchitecture
    {
        void RegisterModel<T>(T instance) where T : IModel;

        void RegisterService<T>(T instance) where T : IService;

        void RegisterUtility<T>(T instance) where T : IUtility;

        T GetModel<T>() where T : IModel;

        T GetService<T>() where T : IService;

        T GetUtility<T>() where T : IUtility;

        void SendCommand<T>() where T : ICommand, new();
        void SendCommand<T>(T command) where T : ICommand;

        Result DoQuery<T, Result>() where T : IQuery<Result>, new();
        Result DoQuery<T, Result>(IQuery<Result> query) where T : IQuery<Result>;
    }
}