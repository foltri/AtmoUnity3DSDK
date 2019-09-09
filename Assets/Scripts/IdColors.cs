using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdColors
{
    private static Dictionary<int, Color> _colors = new Dictionary<int, Color>
    {
        { 0, new Color(1f, 1f, 1f, 1f) },
        { 1, new Color(1f, 1f, 1f, 1f) }
    };

    public static Color GetColor(int uniqueId)
    {
        try
        {
            return _colors[uniqueId];
        }
        catch (KeyNotFoundException)
        {
            return new Color(1f, 1f, 1f, 1f);
        }

    }
}