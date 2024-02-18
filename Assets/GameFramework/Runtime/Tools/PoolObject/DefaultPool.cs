using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;

public class DefaultPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string path;
        public int count;
    }

    public List<Pool> defaultPools = new List<Pool>();

    private void Start()
    {
        List<PoolObject> cache = new List<PoolObject>();
        foreach (var item in this.defaultPools)
        {
            for (int i = 0; i < item.count; i++)
            {
                cache.Add(Daan.ResourceManager.Instance.Spawn<PoolObject>(item.path, this.transform));
            }
        }

        foreach (var item in cache)
        {
            Daan.ResourceManager.Instance.Despawn(item);
        }
    }
}
