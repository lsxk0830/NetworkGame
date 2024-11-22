using System;
using System.Collections.Generic;

namespace SimpleFrame
{
    public class ArchitectureIOC
    {
        private Dictionary<Type, object> mContainer = new Dictionary<Type, object>();

        /// <summary>
        /// 放入容器
        /// </summary>
        public void Push<T>(T instance)
        {
            Type keyType = typeof(T);
            if (mContainer.ContainsKey(keyType))
                mContainer[keyType] = instance;
            else
                mContainer.Add(keyType, instance);
        }

        /// <summary>
        /// 从容器取
        /// </summary>
        public T Pull<T>()
        {
            Type keyType = typeof(T);
            if (mContainer.TryGetValue(keyType, out var inatance))
            {
                return (T)inatance;
            }
            return default;
        }
    }
}