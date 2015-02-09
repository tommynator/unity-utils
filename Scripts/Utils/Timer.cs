using System;
using UnityEngine;

public static class Timer
{
    private static float startTime;
    private static float lapTime;

    public static void Start()
    {
        startTime = lapTime = Time.realtimeSinceStartup;
    }

    public static float Lap(string debug = null)
    {
        float time = 1000 * (Time.realtimeSinceStartup - lapTime);
        lapTime = Time.realtimeSinceStartup;

//        if (!string.IsNullOrEmpty(debug))
//            Debug.Log(string.Format("[{0:f2}ms]: ", time) + debug);

        return time;
    }

    public static float Stop(string tag = null)
    {
        float time = 1000 * (Time.realtimeSinceStartup - startTime);

//        if (string.IsNullOrEmpty(tag))
//            Debug.Log(string.Format("# Finished after {0:f2}ms", time));
//        else
//            Debug.Log(string.Format("# Finished {1} after {0:f2}ms", time, tag));

        return time;
    }
}