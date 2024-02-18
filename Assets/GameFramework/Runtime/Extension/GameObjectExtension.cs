using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
public static partial class GameObjectExtension
{
    public static T MustComponent<T>(this GameObject obj) where T : Component
    {
        T t = obj.GetComponent<T>();

        if (t != null)
        {
            return t;
        }
        return obj.AddComponent<T>();
    }
    public static T MustComponent<T>(this Transform obj) where T : Component
    {
        T t = obj.GetComponent<T>();

        if (t != null)
        {
            return t;
        }
        return obj.gameObject.AddComponent<T>();
    }

    public static Transform MustFind(this Transform obj, string str)
    {
        var result = obj.Find(str);
        if (result)
        {
            return result;
        }
        else
        {
            var newObj = new GameObject(str);
            newObj.transform.SetParent(obj, false);
            return newObj.transform;
        }
    }
    public static void Init(this GameObject obj, string path)
    {
        PoolObject pobj = obj.GetComponent<PoolObject>();
        if (pobj)
        {
            pobj.poolObjectPath = path;
            pobj.Init();
        }
    }

    public static void OnSpawn(this GameObject obj)
    {
        PoolObject pobj = obj.GetComponent<PoolObject>();
        if (pobj) pobj.OnSpawn();
    }

    public static void OnDespawn(this GameObject obj)
    {
        PoolObject pobj = obj.GetComponent<PoolObject>();
        if (pobj) pobj.OnDespawn();
    }

    public static RectTransform GetRectTransform(this GameObject obj)
    {
        return obj.GetComponent<RectTransform>();
    }

    public static void SetLayer(this GameObject obj, int layer, bool recursively = false)
    {
        if (recursively)
        {
            foreach (var item in obj.GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = layer;
            }
        }
        else
        {
            obj.layer = layer;
        }
    }

    public static void SetEnabled(this Behaviour obj, bool boo)
    {
        obj.enabled = boo;
    }

    public static void SetEnabled(this Collider obj, bool boo)
    {
        obj.enabled = boo;
    }

}