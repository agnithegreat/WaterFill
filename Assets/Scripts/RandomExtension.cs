using System.Collections.Generic;

public static class RandomExtension
{
    public static T Random<T>(this T[] list)
    {
        if (list.Length == 0) return default(T);
        var rand = UnityEngine.Random.Range(0, list.Length);
        return list[rand];
    }
    
    public static T Random<T>(this List<T> list)
    {
        if (list.Count == 0) return default(T);
        var rand = UnityEngine.Random.Range(0, list.Count);
        return list[rand];
    }
}