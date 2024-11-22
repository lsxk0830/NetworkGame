using System;
using System.Reflection;

namespace SimpleFrame
{
    public class RuntimeArchitecture
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void ExampleMethod()
        {
            bool isError = false;
            // 获取当前程序集
            Assembly assembly = Assembly.GetExecutingAssembly();
            // 获取抽象泛型类的类型
            Type baseType = typeof(AbstractArchitecture<>);
            // 获取程序集中的所有类型
            Type[] types = assembly.GetTypes();
            // 循环遍历每个类型，检查是否继承自 AbstractTest<T>
            foreach (Type type in types)
            {
                if (type.BaseType != null && type.BaseType.IsGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == typeof(AbstractArchitecture<>))
                {
                    // 创建实例
                    var instance = Activator.CreateInstance(type);
                    // 调用方法
                    MethodInfo method = type.GetMethod("Init");
                    method.Invoke(instance, null);

                    if (isError)
                        UnityEngine.Debug.LogError($"存在多个继承AbstractArchitecture<T>的类,请删除");
                    isError = true;
                }
            }

            /*
            // 获取一个继承AbstractArchitecture<>的类
            // 获取所有程序集中的所有类型
            var types = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes())
                                .Where(t => t.IsClass && !t.IsAbstract); // 仅筛选出具体类

            // 找到继承 AbstractClass<T> 的具体类
            var concreteType = types.FirstOrDefault(t =>
                                t.BaseType != null &&
                                t.BaseType.IsGenericType &&
                                t.BaseType.GetGenericTypeDefinition() == typeof(AbstractArchitecture<>));

            if (concreteType != null)
            {
                // 创建实例
                var instance = Activator.CreateInstance(concreteType);

                // 调用方法
                MethodInfo method = concreteType.GetMethod("Init");
                method.Invoke(instance, null);
            }
            */
        }
    }
}