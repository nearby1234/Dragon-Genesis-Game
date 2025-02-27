using System;
using System.Collections.Generic;
using UnityEngine;

public class ListenerManager : BaseManager<ListenerManager>
{
    private Dictionary<ListenType, ListenerGroup> listeners = new Dictionary<ListenType, ListenerGroup>();

    public void BroadCast(ListenType eventType, object value = null)
    {
        if (listeners.ContainsKey(eventType) && listeners[eventType] != null) 
        { 
            listeners[eventType].BroadCast(value);
        }
    }

    public void Register(ListenType eventType, Action<object> action) 
    {
        if (!listeners.ContainsKey(eventType)) 
        {
            listeners.Add(eventType, new ListenerGroup());
        }

        if (listeners != null) 
        {
            listeners[eventType].Attatch(action);
        }
    }

    public void Unregister(ListenType eventType, Action<object> action)
    {
        if (listeners.ContainsKey(eventType) && listeners[eventType] != null) 
        {
            listeners[eventType].Detatch(action);
        }
    }

    public void UnregisterAll(Action<object> action)
    {
        foreach(ListenType key in listeners.Keys)
        {
            Unregister(key, action);
        }
    }
}
