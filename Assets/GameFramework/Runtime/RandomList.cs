using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
[System.Serializable]
public class RandomList<T> : List<T>
{
    public List<T> cache;
    public RandomList(IEnumerable<T> list) : base(list)
    {

    }

    public RandomList() : base()
    {

    }

    public RandomList(int capacity) : base(capacity)
    {

    }

    public T GetRandom(bool repeat = false, T defaultValue = default)
    {
        this.ResetCache();
        return repeat ? this.GetRandom(pop: false, defaultValue) : cache.GetRandom(pop: true, defaultValue);
    }

    public void ResetCache()
    {
        if (cache == null || cache.Count == 0)
        {
            cache = new List<T>(this);
        }
        else
        {
            //Unity?????????
            if (this.Count == 0 && this.cache.Count != 0)
            {
                this.AddRange(this.cache);
            }
        }
    }
}
