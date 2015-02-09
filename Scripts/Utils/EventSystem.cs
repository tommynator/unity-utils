using System;
using System.Collections.Generic;
using System.Text;

public static class EventSystem
{
    private static Dictionary<string, Delegate> events = new Dictionary<string, Delegate>();

    public static void AddListener(string name, Action action)
    {
        if (!events.ContainsKey(name))
            events[name] = null;
        
        events[name] = Delegate.Combine(events[name], action);
    }
    
    public static void RemoveListener(string name, Action action)
    {
        if (events.ContainsKey(name))
            events[name] = Delegate.Remove(events[name], action);
        
        if(events[name] == null)
            events.Remove(name);
    }

    public static void AddListener<T>(string name, Action<T> action)
    {
        if (!events.ContainsKey(name))
            events[name] = null;

        events[name] = Delegate.Combine(events[name], action);
    }

    public static void RemoveListener<T>(string name, Action<T> action)
    {
        if (events.ContainsKey(name))
            events[name] = Delegate.Remove(events[name], action);
        
        if(events[name] == null)
            events.Remove(name);
    }

    public static void AddListener<T, U>(string name, Action<T, U> action)
    {
        if (!events.ContainsKey(name))
            events[name] = null;
        
        events[name] = Delegate.Combine(events[name], action);
    }
    
    public static void RemoveListener<T, U>(string name, Action<T, U> action)
    {
        if (events.ContainsKey(name))
            events[name] = Delegate.Remove(events[name], action);
        
        if(events[name] == null)
            events.Remove(name);
    }

    public static void Fire(string name, params object[] arg)
    {
        try
        {
            if (events.ContainsKey(name))
            {
                events[name].DynamicInvoke(arg);
            }
        }
        catch(System.Exception e)
        {
            UnityEngine.Debug.LogError("Error invoking event: " + name + "\n" + e);
//            var debug = new StringBuilder("Invocation list:");
//            foreach(var e in events[name].GetInvocationList())
//            {
//                debug.AppendLine((e != null) + " is null");
//            }
//            UnityEngine.Debug.Log(debug);
        }
    }
}