using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentPoint : MonoBehaviour
{
    private Dictionary<string, PartPoint> dict = new Dictionary<string, PartPoint>();

    public PartPoint this[string id]
    {
        get
        {
            return this.dict[id];
        }
        private set { }
    }

    private void Awake()
    {
        foreach (var item in this.GetComponentsInChildren<PartPoint>())
        {
            this.dict.Add(item.name, item);
        }
    }
}
