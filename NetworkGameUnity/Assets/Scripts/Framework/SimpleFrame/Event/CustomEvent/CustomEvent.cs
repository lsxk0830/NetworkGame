using System;

namespace SimpleFrame
{
    public class CustomEvent<T> : ICustomEvent where T : IEvent
    {
        private Action<T> mOnEvent = (e) => { };

        public void RegisterEvent(Action<T> onEvent)
        {
            mOnEvent += onEvent;
        }

        public void UnRegisterEvent(Action<T> onEvent)
        {
            mOnEvent -= onEvent;
        }

        public void InvokeEvent(T onEvent)
        {
            mOnEvent?.Invoke(onEvent);
        }
    }
}