using System;
using UnityEngine;

public static class MathUtil
{
    const float a = 1;
    const float b = 2;

    public static float Polar2Ellipsis(float polar)
    {
        return a * b / Mathf.Sqrt(Mathf.Pow(b * Mathf.Cos(polar), 2) + Mathf.Pow(a * Mathf.Sin(polar), 2));
    }
}