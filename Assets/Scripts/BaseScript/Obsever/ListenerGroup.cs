using System;
using System.Collections.Generic;

public class ListenerGroup
{
    List<Action<object>> actions = new();

    private object stickyValue = null;
    private bool hasStickyValue = false;
    public void BroadCast(object value)
    {
        stickyValue = value;
        hasStickyValue = true;
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i](value);
        }
    }

    public void Attatch(Action<object> action) 
    {
        if (actions.Contains(action))
            return;

        //for (int i = 0; i < actions.Count; i++) 
        //{
        //    if (actions[i] == action)
        //        return;
        //}

        actions.Add(action);
        // Nếu có sticky value, gọi callback ngay
        if (hasStickyValue)
        {
            action(stickyValue);
        }
    }

    public void Detatch(Action<object> action) 
    {
        //for (int i = 0; i < actions.Count; i++) 
        //{ 
        //    if(actions[i] == action)
        //    {
        //        actions.Remove(action);
        //        break;
        //    }
        //}
        if (actions.Contains(action))
        {
            actions.Remove(action);
        }
    }
}
