using System;
using UnityEngine;

namespace SimpleFrame
{
    public static class CanEventExtension
    {
        private static EventIOC Global = new EventIOC();

        /// <summary>
        /// 取消注册事件
        /// </summary>
        public static Action<T> RegisterEvent<T>(this ICanEvent self, Action<T> onEvent) where T : IEvent
        {
            Global.RegisterEvent<T>(onEvent);
            return onEvent;
        }

        /// <summary>
        /// 取消注册事件
        /// </summary>
        public static void UnRegisterEvent<T>(this ICanEvent self, Action<T> onEvent) where T : IEvent
        {
            Global.UnRegisterEvent<T>(onEvent);
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        public static void TriggerEvent<T>(this ICanEvent self, T instance) where T : IEvent
        {
            Global.InvokeEvent<T>(instance);
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        public static void TriggerEvent<T>(this ICanEvent self) where T : IEvent, new()
        {
            Global.InvokeEvent<T>();
        }

        /// <summary>
        /// 指定物体OnEnable时注册事件、OnDisable取消注册事件
        /// </summary>
        public static void UnRegisterEventWithGameObjectDisable<T>(this Action<T> self, GameObject go) where T : IEvent, new()
        {
            if (go == null)
            {
                Debug.LogError($"绑定的物体为空");
                return;
            }
            if (!go.TryGetComponent(out UnRegisterEventWithGameObjectDisable UnRegisterEventWithGameObjectDisable))
            {
                UnRegisterEventWithGameObjectDisable = go.AddComponent<UnRegisterEventWithGameObjectDisable>();
                UnRegisterEventWithGameObjectDisable.Init(Global);
            }
            UnRegisterEventWithGameObjectDisable.AddUnRegisterEventAction(self);
        }

        /// <summary>
        /// 指定物体OnDestroy时取消注册事件
        /// </summary>
        public static void UnRegisterEventWithGameObjectDestroy<T>(this Action<T> self, GameObject go) where T : IEvent, new()
        {
            if (go == null)
            {
                Debug.LogError($"绑定的物体为空");
                return;
            }
            if (!go.TryGetComponent(out UnRegisterEventWithGameObjectDestroy UnRegisterEventWithGameObjectDestroy))
            {
                UnRegisterEventWithGameObjectDestroy = go.AddComponent<UnRegisterEventWithGameObjectDestroy>();
                UnRegisterEventWithGameObjectDestroy.Init(Global);
            }
            UnRegisterEventWithGameObjectDestroy.AddUnRegisterEventAction(self);
        }
    }
}