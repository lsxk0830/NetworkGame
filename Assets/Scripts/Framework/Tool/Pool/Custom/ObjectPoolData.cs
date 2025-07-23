using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 普通类 对象 对象池数据
/// </summary>
public class ObjectPoolData<T> : IPoolData
{
    /// <summary>
    /// 对象容器
    /// </summary>
    public Stack<T> PoolStack = new Stack<T>();

    /// <summary>
    /// 将对象放进对象池
    /// </summary>
    /// <param name="obj">具体某个类型的实例</param>
    public void PushObj(T obj)
    {
        PoolStack.Push(obj);
        //Debug.Log($"数量:{PoolStack.Count}");
    }

    /// <summary>
    /// 从对象池中获取对象,取栈顶元素
    /// </summary>
    /// <returns></returns>
    public T GetObj()
    {
        T obj = PoolStack.Pop();
        return obj;
    }

    /// <summary>
    /// 清空此对象的对象池数据
    /// </summary>
    public void Clear()
    {
        PoolStack.Clear();
        this.PushPool(this);
    }
}