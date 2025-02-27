using System;
using System.Collections.Generic;

public class ListenerGroup
{
    List<Action<object>> actions = new List<Action<object>>();

    public void BroadCast(object value)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i](value);
        }
    }

    public void Attatch(Action<object> action) 
    {
        for (int i = 0; i < actions.Count; i++) 
        {
            if (actions[i] == action)
                return;
        }

        actions.Add(action);
    }

    public void Detatch(Action<object> action) 
    {
        for (int i = 0; i < actions.Count; i++) 
        { 
            if(actions[i] == action)
            {
                actions.Remove(action);
                break;
            }
        }
    }
}
