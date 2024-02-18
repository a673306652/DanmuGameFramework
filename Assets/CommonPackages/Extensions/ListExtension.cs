using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class ListExtension
{

    public static T GetFirst<T>(this List<T> list, bool pop = false)
    {
        if (list.Count == 0)
            return default(T);
        var result = list[0];
        if (pop)
        {
            list.RemoveAt(0);
        }
        return result;
    }
    public static T GetLast<T>(this List<T> list, bool pop = false)
    {
        if (list.Count - 1 >= 0)
        {
            var result = list[list.Count - 1];
            if (pop)
            {
                list.RemoveAt(list.Count - 1);
            }
            return result;
        }
        else
            return default(T);
    }
    public static void RandomSort<T>(this List<T> list)
    {
        var temp = new List<T>();
        for (int i = 0; i < list.Count; i++)
        {
            var item = list.GetRandom();
            if (item == null) continue;

            temp.Add(item);
            list.Remove(item);
            i--;
        }
        list = temp;
    }

    public static T GetRandom<T>(this List<T> list, bool pop = false,T defaultValue = default)
    {
        if (list.Count == 0)
            return defaultValue;
        int rnd = Random.Range(0, list.Count);
        var result = list[rnd];
        if (pop)
        {
            list.RemoveAt(rnd);
        }
        return result;
    }
    public static T GetRandom<T>(this List<T> list, System.Random random, bool pop = false)
    {
        if (list.Count == 0)
            return default(T);
        int rnd = random.Next(list.Count);
        var result = list[rnd];
        if (pop)
        {
            list.RemoveAt(rnd);
        }
        return result;
    }
    public static T TryGet<T>(this List<T> list, int i)
    {
        if (i >= 0 && list.Count > 0 && i < list.Count)
        {
            return list[i];
        }
        return default(T);
    }

    public static K GetRandomKey<K, V>(this Dictionary<K, V> dict)
    {
        if (dict.Count == 0)
        {
            return default(K);
        }
        var list = new List<K>(dict.Keys);
        return list.GetRandom();
    }
    public static V GetRandomValue<K, V>(this Dictionary<K, V> dict)
    {
        if (dict.Count == 0)
        {
            return default(V);
        }
        var list = new List<V>(dict.Values);
        return list.GetRandom();
    }
    public static V TryGet<K, V>(this Dictionary<K, V> dict, K key, V defaultValue = default(V))
    {
        if (dict.TryGetValue(key, out var get))
        {
            return get;
        }
        else
        {
            return defaultValue;
        }
    }

    public static void TryAdd<K, V>(this Dictionary<K, V> dict, K key, V value)
    {
        if (!dict.ContainsKey(key))
        {
            dict.Add(key, value);
        }
        else
        {
            Debug.LogError("重复添加元素");
        }
    }
}
