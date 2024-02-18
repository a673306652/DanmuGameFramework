using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntProperty
{
    public int value;
    public int max;
    public int origin;

    public IntProperty(int origin)
    {
        this.value = this.max = this.origin = origin;
    }

    public IntProperty(int origin, int max)
    {
        this.value = this.origin = origin;
        this.max = max;
    }

    public void Reset()
    {
        this.value = this.origin;
    }
}
