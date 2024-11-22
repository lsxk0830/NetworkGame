using System;
using UnityEngine;

namespace SimpleFrame
{
    /// <summary>
    /// 对象池扩展
    /// </summary>
    public static class PoolExtension
    {
        private static ObjectPoolIOC ObjGlobal = new ObjectPoolIOC();
        private static GameObjectPoolIOC goGlobal = new GameObjectPoolIOC();

        #region Object
        #region 放入对象

        /// <summary>
        /// 对象放入对象池
        /// </summary>
        /// <param name="objFullName">放入对象池时对象对应的FullName</param>
        /// <param name="instance">对象实例</param>
        public static void PushPool<T>(this object obj, T instance)
        {
            ObjGlobal.PushPool<T>(instance);
        }

        /// <summary>
        /// 对象放入对象池
        /// </summary>
        /// <param name="objFullName">放入对象池时对象对应的FullName</param>
        /// <param name="instance">对象实例</param>
        public static void PushPool<T>(this object obj, string objFullName, T instance) where T : struct
        {
            ObjGlobal.PushPool<T>(objFullName, instance);
        }

        #endregion
        #region 获取对象

        /// <summary>
        /// 获取对象，没有则生成
        /// </summary>
        /// <typeparam name="T">指定类型对象</typeparam>
        /// <returns>指定类型对象</returns>
        public static T GetObjInstance<T>(this object obj) where T : new()
        {
            return ObjGlobal.GetObjInstance<T>();
        }

        /// <summary>
        /// 获取对象，没有则生成
        /// </summary>
        /// <typeparam name="T">指定类型对象</typeparam>
        /// <returns>指定类型对象</returns>
        public static T GetObjInstance<T>(this object obj, string objFullName) where T : struct
        {
            return ObjGlobal.GetObjInstance<T>(objFullName);
        }

        /// <summary>
        /// 获取获取类对象，有参，没有则生成
        /// </summary>
        /// <typeparam name="T">指定类型对象</typeparam>
        /// <returns>指定类型对象</returns>
        public static T GetObjInstance<T>(this object obj, params object[] objList) where T : class
        {
            return ObjGlobal.GetObjInstance<T>(objList);
        }

        /// <summary>
        /// 获取获取类对象，有参，没有则生成
        /// </summary>
        /// <typeparam name="T">指定类型对象</typeparam>
        /// <returns>指定类型对象</returns>
        public static T GetObjInstance<T>(this object obj, string objFullName, params object[] objList) where T : class
        {
            return ObjGlobal.GetObjInstance<T>(objFullName, objList);
        }

        #endregion
        #region 清空对象池

        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public static void ClearAll(this object obj)
        {
            ObjGlobal.ClearAll();
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        /// <typeparam name="T">具体某个类型</typeparam>
        public static void ClearPool<T>(this object obj) where T : struct
        {
            ObjGlobal.ClearPool<T>();
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        /// <param name="type">具体某个类型</param>
        public static void ClearPool<T>(this object obj, Type type) where T : struct
        {
            ObjGlobal.ClearPool<T>(type);
        }

        /// <summary>
        /// 清空结构体对象池
        /// </summary>
        /// <param name="type">具体某个类型</param>
        public static void ClearPool<T>(this object obj, string objFullName) where T : struct
        {
            ObjGlobal.ClearPool<T>(objFullName);
        }

        #endregion
        #endregion
        #region GameObject
        #region 放入对象
        /// <summary>
        /// 对象放入对象池
        /// </summary>
        /// <param name="obj">对象</param>
        public static void PushGameObject(this object self, GameObject go)
        {
            goGlobal.PushGameObject(go);
        }
        #endregion
        #region 获取对象

        /// <summary>
        /// 获取GameObject
        /// </summary>
        /// <param name="prefab">要获取的对象</param>
        /// <param name="parent">父物体</param>
        /// <returns></returns>
        public static GameObject GetGameObject(this object self, GameObject prefab, Transform parent = null)
        {
            return goGlobal.GetGameObject(prefab, parent);
        }

        #endregion
        #region 清空对象池

        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public static void ClearGameobjectPool(this GameObject self)
        {
            goGlobal.ClearGameobjectPool();
        }

        #endregion
        #endregion
    }
}