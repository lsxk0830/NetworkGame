namespace SimpleFrame
{
    /// <summary>
    /// 查询接口，所有查询都必须继承 IQuery<T> 接口
    /// </summary>
    /// <typeparam name="T">查询的返回值类型</typeparam>
    public interface IQuery<T> : ICanGetModel, ICanGetService, ICanGetUtility
    {
        T Query();
    }
}