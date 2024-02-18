using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionListener : MonoBehaviour
{
    public delegate void CollisionDelegate(CollisionEvent self, Collision other);
    public delegate void ColliderDelegate(CollisionEvent self, Collider other);

    private Dictionary<int, Dictionary<string, CollisionDelegate>> delegateDict = new Dictionary<int, Dictionary<string, CollisionDelegate>>();
    private Dictionary<int, Dictionary<string, ColliderDelegate>> delegateDict2 = new Dictionary<int, Dictionary<string, ColliderDelegate>>();


    public void Register(string eventName, CollisionDelegate d, int type = 0)
    {
        if (!this.delegateDict.ContainsKey(type)) this.delegateDict.Add(type, new Dictionary<string, CollisionDelegate>());
        var dict = this.delegateDict[type];
        if (!dict.ContainsKey(eventName)) dict.Add(eventName, null);
        dict[eventName] += d;
    }

    public void Register(string eventName, ColliderDelegate d, int type = 0)
    {
        if (!this.delegateDict2.ContainsKey(type)) this.delegateDict2.Add(type, new Dictionary<string, ColliderDelegate>());
        var dict = this.delegateDict2[type];
        if (!dict.ContainsKey(eventName)) dict.Add(eventName, null);
        dict[eventName] += d;
    }

    public void Excute(string eventName, CollisionEvent self, Collision other, int type = 0)
    {
        if (!this.delegateDict.ContainsKey(type)) this.delegateDict.Add(type, new Dictionary<string, CollisionDelegate>());
        var dict = this.delegateDict[type];
        if (!dict.ContainsKey(eventName)) return;
        dict[eventName].Invoke(self,other);
    }

    public void Excute(string eventName, CollisionEvent self, Collider other, int type = 0)
    {
        if (!this.delegateDict2.ContainsKey(type)) this.delegateDict2.Add(type, new Dictionary<string, ColliderDelegate>());
        var dict = this.delegateDict2[type];
        if (!dict.ContainsKey(eventName)) return;
        dict[eventName].Invoke(self, other);
    }

    public void Clear()
    {
        this.delegateDict.Clear();
        this.delegateDict2.Clear();
    }
}
