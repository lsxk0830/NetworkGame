using System.Collections.Generic;

namespace SimpleFrame
{
    public abstract class AbstractArchitecture<T> : IArchitecture where T : AbstractArchitecture<T>, new()
    {
        private ArchitectureIOC mIOCContainer = new ArchitectureIOC();
        private HashSet<IService> mServiceList = new HashSet<IService>();
        private HashSet<IModel> mModelList = new HashSet<IModel>();
        public void Init()
        {
            CanGetModelExtension.SetArchitecture(this);
            CanGetServiceExtension.SetArchitecture(this);
            CanGetUtilityExtension.SetArchitecture(this);
            CanSendCommandExtension.SetArchitecture(this);
            CanDoQueryExtension.SetArchitecture(this);

            OnInit();

            foreach (var model in mModelList)
                model.Init();
            foreach (var service in mServiceList)
                service.Init();

            // 注入
            foreach (var model in mModelList)
                DIContainer.InjectDependencies(model);
            foreach (var service in mServiceList)
                DIContainer.InjectDependencies(service);
        }

        protected abstract void OnInit();

        public void RegisterModel<TModel>(TModel instance) where TModel : IModel
        {
            mModelList.Add(instance);
            mIOCContainer.Push<TModel>(instance);
            DIContainer.Register(instance);
        }

        public void RegisterService<TService>(TService instance) where TService : IService
        {
            mServiceList.Add(instance);
            mIOCContainer.Push<TService>(instance);
            DIContainer.Register(instance);
        }

        public void RegisterUtility<TUtility>(TUtility instance) where TUtility : IUtility
        {
            mIOCContainer.Push<TUtility>(instance);
            DIContainer.Register(instance);
        }

        public TModel GetModel<TModel>() where TModel : IModel
        {
            return mIOCContainer.Pull<TModel>();
        }

        public TService GetService<TService>() where TService : IService
        {
            return mIOCContainer.Pull<TService>();
        }

        public TUtility GetUtility<TUtility>() where TUtility : IUtility
        {
            return mIOCContainer.Pull<TUtility>();
        }

        public void SendCommand<TCommand>() where TCommand : ICommand, new()
        {
            SendCommand<TCommand>(this.GetObjInstance<TCommand>());
        }

        public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            command.Execute();
            this.PushPool(command);
        }

        public Result DoQuery<TQuery, Result>() where TQuery : IQuery<Result>, new()
        {
            return DoQuery<TQuery, Result>(this.GetObjInstance<TQuery>());
        }
        public Result DoQuery<TQuery, Result>(IQuery<Result> query) where TQuery : IQuery<Result>
        {
            Result result = query.Query();
            this.PushPool(query);
            return result;
        }
    }
}