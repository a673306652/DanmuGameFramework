using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;

public class RandomDictionary<K, V> : Dictionary<K, V>
{
    public RandomList<K> keyList = new RandomList<K>();
    public RandomList<V> valueList = new RandomList<V>();

    public RandomDictionary() : base()
    {

    }

    public K GetRandomKey(bool repeat, K defaultValue = default)
    {
        if (this.Count == 0)
            return defaultValue;
        if (repeat)
        {
            return this.GetRandomKey();
        }
        else
        {
            K k = default;
            bool boo = true;
            while (boo)
            {
                if (keyList == null || keyList.cache == null || keyList.cache.Count == 0)
                {
                    keyList = new RandomList<K>(this.Keys);
                }
                k = keyList.GetRandom();
                if (this.ContainsKey(k))
                {
                    boo = false;
                }
            }
            return k;
        }
    }

    public V GetRandomValue(bool repeat, V defaultValue = default)
    {
        if (this.Count == 0)
            return defaultValue;
        if (repeat)
        {
            return this.GetRandomValue();
        }
        else
        {
            V v = default;
            bool boo = true;
            while (boo)
            {
                if (valueList == null || valueList.cache == null || valueList.cache.Count == 0)
                {
                    valueList = new RandomList<V>(this.Values);
                }
                v = valueList.GetRandom();
                if (this.ContainsValue(v))
                {
                    boo = false;
                }
            }
            return v;
        }
    }
}
