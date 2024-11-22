using System;
using System.Collections.Generic;

namespace SimpleFrame
{
    public class EventIOC
    {
        private Dictionary<Type, ICustomEvent> EventDic = new Dictionary<Type, ICustomEvent>();
        public void RegisterEvent<T>(Action<T> onEvent) where T : IEvent
        {
            Type type = typeof(T);
            if (!EventDic.ContainsKey(type))
            {
                CustomEvent<T> easy = this.GetObjInstance<CustomEvent<T>>();
                EventDic[type] = easy;
                easy.RegisterEvent(onEvent);
            }
            else
                (EventDic[type] as CustomEvent<T>).RegisterEvent(onEvent);
        }

        public void UnRegisterEvent<T>(Action<T> onEvent) where T : IEvent
        {
            Type type = typeof(T);
            if (EventDic.TryGetValue(type, out ICustomEvent customEvent))
            {
                (EventDic[type] as CustomEvent<T>).UnRegisterEvent(onEvent);
            }
        }

        public void InvokeEvent<T>(T onEvent) where T : IEvent
        {
            Type type = typeof(T);
            if (EventDic.TryGetValue(type, out ICustomEvent customEvent))
            {
                (EventDic[type] as CustomEvent<T>).InvokeEvent(onEvent);
            }
        }

        public void InvokeEvent<T>() where T : IEvent, new()
        {
            Type type = typeof(T);
            if (EventDic.TryGetValue(type, out ICustomEvent customEvent))
            {
                (EventDic[type] as CustomEvent<T>).InvokeEvent(this.GetObjInstance<T>());
            }
        }
    }
}