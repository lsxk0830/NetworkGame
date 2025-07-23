using System;
using System.Collections.Generic;

public class ObjectPoolIOC
{
    /// <summary>
    /// 普通类 对象容器
    /// </summary>
    private Dictionary<string, IPoolData> PoolDic = new Dictionary<string, IPoolData>();

    #region 放入对象

    /// <summary>
    /// 对象放入对象池
    /// </summary>
    /// <param name="objFullName">放入对象池时对象对应的FullName</param>
    /// <param name="instance">对象实例</param>
    public void PushPool<T>(T instance)
    {
        PushPool<T>(typeof(T).FullName, instance);
    }

    /// <summary>
    /// 对象放入对象池
    /// </summary>
    /// <param name="objFullName">放入对象池时对象对应的FullName</param>
    /// <param name="instance">对象实例</param>
    public void PushPool<T>(string objFullName, T instance)
    {
        if (instance.InstanceIsNull())
        {
            UnityEngine.Debug.LogError($"放入对象池的对象:{typeof(T).FullName}的实例为空");
            return;
        }

        if (!PoolDic.TryGetValue(objFullName, out IPoolData poolData))
        {
            poolData = CreateObjectPoolData<T>(objFullName);
        }
        (poolData as ObjectPoolData<T>).PushObj(instance);

    }

    #endregion

    #region 获取对象

    /// <summary>
    /// 获取对象实例，没有则生成
    /// </summary>
    /// <typeparam name="T">指定类型对象</typeparam>
    /// <returns>指定类型对象</returns>
    public T GetObjInstance<T>() where T : new()
    {
        return GetObjInstance<T>(typeof(T).FullName);
    }

    /// <summary>
    /// 获取对象实例，没有则生成
    /// </summary>
    /// <typeparam name="T">指定类型对象</typeparam>
    /// <returns>指定类型对象</returns>
    public T GetObjInstance<T>(string objFullName) where T : new()
    {
        if (PoolDic.ContainsKey(objFullName) && (PoolDic[objFullName] as ObjectPoolData<T>).PoolStack.Count > 0)
            return (PoolDic[objFullName] as ObjectPoolData<T>).GetObj();
        else
            return new T();
    }

    /// <summary>
    /// 获取获取类对象，有参，没有则生成
    /// </summary>
    /// <typeparam name="T">指定类型对象</typeparam>
    /// <returns>指定类型对象</returns>
    public T GetObjInstance<T>(params object[] obj) where T : class
    {
        return GetObjInstance<T>(typeof(T).FullName, obj);
    }

    /// <summary>
    /// 获取获取类对象，有参，没有则生成
    /// </summary>
    /// <typeparam name="T">指定类型对象</typeparam>
    /// <returns>指定类型对象</returns>
    public T GetObjInstance<T>(string objFullName, params object[] objList) where T : class
    {
        if (PoolDic.ContainsKey(objFullName) && (PoolDic[objFullName] as ObjectPoolData<T>).PoolStack.Count > 0)
            return (PoolDic[objFullName] as ObjectPoolData<T>).GetObj();
        else
            return Activator.CreateInstance(typeof(T), objList) as T;
    }

    #endregion

    #region 清空对象池

    /// <summary>
    /// 清空所有对象池
    /// </summary>
    public void ClearAll()
    {
        var enumerator = PoolDic.GetEnumerator();
        while (enumerator.MoveNext())
            enumerator.Current.Value.Clear(); ;
        PoolDic.Clear();
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    /// <typeparam name="T">具体某个类型</typeparam>
    public void ClearPool<T>()
    {
        ClearPool<T>(typeof(T).FullName);
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    /// <param name="type">具体某个类型</param>
    public void ClearPool<T>(Type type)
    {
        ClearPool<T>(type.FullName);
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    /// <param name="type">具体某个类型</param>
    public void ClearPool<T>(string objFullName)
    {
        if (PoolDic.TryGetValue(objFullName, out IPoolData PoolData))
        {
            PoolData.Clear();
            PoolDic.Remove(objFullName);
        }
    }

    #endregion

    #region 创建对象池数据

    /// <summary>
    /// 创建一条新的对象池数据_结构体型
    /// </summary>
    private IPoolData CreateObjectPoolData<T>(string objFullName)
    {
        IPoolData poolData = new ObjectPoolData<T>();
        PoolDic.Add(objFullName, poolData);
        return poolData;
    }

    #endregion
}