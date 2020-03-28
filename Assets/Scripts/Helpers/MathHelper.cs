using UnityEngine;
using UnityEditor;

public class MathHelper 
{
    /// <summary>
    /// Returns true if value is between min and max
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static bool IsWithin(double value, double min, double max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Returns true if value1 is in [value2-interval, value2+interval]
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static bool IsNearby(double value1, double value2, double interval)
    {
        return IsWithin(value1, value2 - interval, value2 + interval);
    }
   
}