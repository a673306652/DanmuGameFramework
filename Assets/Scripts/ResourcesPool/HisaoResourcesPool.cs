using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HisaoResourcesPool : SingletonForMonoBehaviour<HisaoResourcesPool>
{
    private Dictionary<string, HisaoObjectPool> _objectPools;

    private void Awake()
    {
        _objectPools = new Dictionary<string, HisaoObjectPool>();
    }

    public GameObject Load(string url, int preGenerateCount = 0)
    {
        if (_objectPools.ContainsKey(url))
        {
            return _objectPools[url].GetOne();
        }
        else
        {
            NewPool(url, preGenerateCount);
            return _objectPools[url].GetOne();
        }
    }

    private void NewPool(string url, int preGenerateCount)
    {
        var x = new GameObject();
        x.transform.SetParent(this.transform);
        x.name = url;
        var c = x.GetOrAddComponent<HisaoObjectPool>();
        var template = Resources.Load<GameObject>(url);
        if (null == template)
        {
            Debug.LogError("构建池失败，目标路径" + url + "无实际对象");
        }

        c.Init(template, preGenerateCount);
        _objectPools.Add(url, c);
    }

    public void ReturnOne(string url, GameObject obj)
    {
        var p = _objectPools[url];
        if (null != p)
        {
            p.ReturnOne(obj);
        }
    }
}