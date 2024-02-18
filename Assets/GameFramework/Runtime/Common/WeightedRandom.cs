using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WeightedRandom<T>
{
    public int weight;
    private T _result;
    public T Result
    {
        get
        {
            return _result;
        }
    }
    public WeightedRandom(int w, T t)
    {
        this.weight = w;
        this._result = t;

    }


    /// <summary>
    /// 无视权重随机一个元素 PS：自带随机器
    /// </summary>
    public static WeightedRandom<T> GetRandomItem(System.Random random, ICollection<WeightedRandom<T>> collection)
    {
        return GetRandomItem(random, collection, GetTotalWeight(collection));
    }

    /// <summary>
    /// 指定权重随机一个元素 PS：自带随机器
    /// </summary>
    public static WeightedRandom<T> GetRandomItem(System.Random random, ICollection<WeightedRandom<T>> collection, int targetWeight)
    {
        if (targetWeight <= 0)
        {
            Debug.LogWarning("无法随机一个没有权重的元素");
            return null;
        }
        else
        {
            int rnd = -1;
            if (random == null)
            {
                rnd = UnityEngine.Random.Range(0, targetWeight);
            }
            else
            {
                rnd = random.Next(targetWeight);
            }
            return GetRandomItem(collection, rnd);
        }
    }
    /// <summary>
    /// 指定权重随机一个元素
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="weight"></param>
    /// <returns></returns>
    public static WeightedRandom<T> GetRandomItem(ICollection<WeightedRandom<T>> collection, int weight)
    {
        IEnumerator<WeightedRandom<T>> ie = collection.GetEnumerator();
        WeightedRandom<T> item;

        do
        {
            if (!ie.MoveNext())
            {
                return null;
            }

            item = ie.Current;
            weight -= item.weight;
        } while (weight >= 0);
        return item;
    }
    public static int GetTotalWeight(ICollection<WeightedRandom<T>> collection)
    {
        int value = 0;
        IEnumerator<WeightedRandom<T>> ie = collection.GetEnumerator();
        while (ie.MoveNext())
        {
            value += ie.Current.weight;
        }
        return value;
    }

}


