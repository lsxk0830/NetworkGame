using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 普通类 对象 对象池数据
/// </summary>
public class ObjectPoolData<T> : IPoolData
{
    /// <summary>
    /// 对象容器
    /// </summary>
    public HashSet<T> PoolQueue = new HashSet<T>();

    /// <summary>
    /// 将对象放进对象池
    /// </summary>
    /// <param name="obj">具体某个类型的实例</param>
    public void PushObj(T obj)
    {
        PoolQueue.Add(obj);
    }

    /// <summary>
    /// 从对象池中获取对象,取栈顶元素
    /// </summary>
    /// <returns></returns>
    public T GetObj()
    {
        T t = PoolQueue.ElementAt(0);
        PoolQueue.Remove(t);
        return t;
    }

    /// <summary>
    /// 清空此对象的对象池数据
    /// </summary>
    void IPoolData.Clear()
    {
        PoolQueue.Clear();
        this.PushPool(this);
    }
}