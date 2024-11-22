using System;
using UnityEngine;

namespace SimpleFrame
{
    /// <summary>
    /// 指定物体OnDestroy时取消注册事件
    /// </summary>
    public class UnRegisterEventWithGameObjectDestroy : MonoBehaviour
    {
        private Action UnRegisterEvents;
        private EventIOC Global;

        public void Init(EventIOC EventIOC)
        {
            Global = EventIOC;
        }

        public void AddUnRegisterEventAction<T>(Action<T> action) where T : IEvent
        {
            UnRegisterEvents += () =>
            {
                Global.UnRegisterEvent<T>(action);
            };
        }

        private void OnDestroy() => UnRegisterEvents?.Invoke();
    }
}