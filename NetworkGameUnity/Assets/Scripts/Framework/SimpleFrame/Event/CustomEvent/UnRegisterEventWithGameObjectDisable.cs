using System;
using UnityEngine;

namespace SimpleFrame
{
    /// <summary>
    /// 指定物体OnEnable时注册事件、OnDisable取消注册事件
    /// </summary>
    public class UnRegisterEventWithGameObjectDisable : MonoBehaviour
    {
        private Action RegisterEvents;
        private Action UnRegisterEvents;
        private EventIOC Global;

        public void Init(EventIOC EventIOC)
        {
            Global = EventIOC;
        }

        public void AddUnRegisterEventAction<T>(Action<T> action) where T : IEvent
        {
            RegisterEvents += () =>
            {
                Global.RegisterEvent<T>(action);
            };

            UnRegisterEvents += () =>
            {
                Global.UnRegisterEvent<T>(action);
            };
        }

        private bool isEventsRegistered = true;

        private void OnEnable()
        {
            if (!isEventsRegistered)
            {
                RegisterEvents?.Invoke();
                isEventsRegistered = true;
            }
        }

        private void OnDisable()
        {
            if (isEventsRegistered)
            {
                UnRegisterEvents?.Invoke();
                isEventsRegistered = false;
            }
        }
    }
}