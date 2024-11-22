using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleFrame
{
    public static class DIContainer
    {
        private static Dictionary<Type, object> instances = new Dictionary<Type, object>();

        // 注册实例
        public static void Register<T>(T instance)
        {
            var type = typeof(T);
            if (!instances.ContainsKey(type))
            {
                instances[type] = instance;
            }
        }

        // 注入依赖
        public static void InjectDependencies(object obj)
        {
            var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var injectAttr = field.GetCustomAttributes(typeof(InjectAttribute), true);
                if (injectAttr.Length > 0)
                {
                    var fieldType = field.FieldType;
                    object fieldValue = null;

                    if (instances.ContainsKey(fieldType))
                    {
                        fieldValue = instances[fieldType];
                    }
                    else
                    {
                        fieldValue = Activator.CreateInstance(fieldType);
                        instances[fieldType] = fieldValue;
                    }

                    field.SetValue(obj, fieldValue);
                }
            }

            foreach (var property in properties)
            {
                var injectAttr = property.GetCustomAttributes(typeof(InjectAttribute), true);
                if (injectAttr.Length > 0 && property.CanWrite)
                {
                    var propertyType = property.PropertyType;
                    object propertyValue = null;

                    if (instances.ContainsKey(propertyType))
                    {
                        propertyValue = instances[propertyType];
                    }
                    else
                    {
                        propertyValue = Activator.CreateInstance(propertyType);
                        instances[propertyType] = propertyValue;
                    }

                    property.SetValue(obj, propertyValue);
                }
            }
        }
    }
}