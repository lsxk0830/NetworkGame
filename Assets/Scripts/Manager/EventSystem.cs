using System;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    public Dictionary<string, Action> Container = new Dictionary<string, Action>();
    public Dictionary<string, Action<string>> ContainerStr = new Dictionary<string, Action<string>>();
    public Dictionary<string, Action<MsgBase>> ContainerMsgBase = new Dictionary<string, Action<MsgBase>>();

    public void RegisterEvent(string eventName, Action callback)
    {
        if (Container.ContainsKey(eventName))
            Container[eventName] += callback;
        else
            Container[eventName] = callback;
    }

    public void RemoveEvent(string eventName, Action callback)
    {
        if (Container.ContainsKey(eventName))
            Container[eventName] -= callback;
    }

    public void InvokeEvent(string eventName)
    {
        if (Container.ContainsKey(eventName))
            Container[eventName]?.Invoke();
    }

    public void RegisterEvent(string eventName, Action<string> callback)
    {
        if (ContainerStr.ContainsKey(eventName))
            ContainerStr[eventName] += callback;
        else
            ContainerStr[eventName] = callback;
    }

    public void RemoveEvent(string eventName, Action<string> callback)
    {
        if (ContainerStr.ContainsKey(eventName))
            ContainerStr[eventName] -= callback;
    }

    public void InvokeEvent(string eventName, string message)
    {
        if (ContainerStr.ContainsKey(eventName))
            ContainerStr[eventName]?.Invoke(message);
    }

    public void RegisterEvent(string eventName, Action<MsgBase> callback)
    {
        if (ContainerMsgBase.ContainsKey(eventName))
            ContainerMsgBase[eventName] += callback;
        else
            ContainerMsgBase[eventName] = callback;
    }

    public void RemoveEvent(string eventName, Action<MsgBase> callback)
    {
        if (ContainerMsgBase.ContainsKey(eventName))
            ContainerMsgBase[eventName] -= callback;
    }

    public void InvokeEvent(string eventName, MsgBase message)
    {
        if (ContainerMsgBase.ContainsKey(eventName))
            ContainerMsgBase[eventName]?.Invoke(message);
    }
}