using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeData
{
    public float origin;
    [HideInInspector]
    public float add = 0;
    [HideInInspector]
    public float multi = 0;
    [HideInInspector]
    public float? fix = null;

    public float value
    {
        private set { }
        get
        {
            return this.fix != null ? this.fix.Value : (this.origin + this.add) * (1 + this.multi);
        }
    }

    public AttributeData(float origin)
    {
        this.Clear();
        this.origin = origin;
    }

    public void Clear()
    {
        this.add = 0;
        this.multi = 0;
        this.fix = null;
    }
}
