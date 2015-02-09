using UnityEngine;
using System.Collections;

public static class ColorCode
{
    private static Color[] colors = new Color[] { new Color(1, 0, 0),  new Color(0.5f, 0.5f, 0.5f),  new Color(0, 1, 0),  new Color(0, 0, 1),  new Color(1, 0.5f, 0.5f),  new Color(0, 0.5f, 0.5f),  new Color(0.5f, 0, 0.5f),  new Color(1, 0.5f, 1) };

    public static Color Palette(int index)
    {
        return colors[index%colors.Length];
    }

    static ColorCode()
    {
        for(int i = 0; i < 2000; i += 11)
        {
            if(ToIndex(ToColor(i)) != i)
            {
                Debug.LogError("Test failed with: " + i);
                break;
            }
        }
    }

    public static Color32 ToColor(int index)
    {
        byte r = (byte)(index % 255);
        byte g = (byte)((index - r) / 255);
        return new Color32( r, g, 0, 0);
    }

    public static int ToIndex(Color32 color)
    {
        return color.r + color.g * 255;
    }
}
