using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloatProperty
{
    [SerializeField, SetProperty("value")]
    private float _value;
    public float value
    {
        set
        {
            this._value = Mathf.Min(this.max, value);
        }
        get { return this._value; }
    }
    public float max;
    public float origin;

    public FloatProperty(float origin)
    {
        this.value = this.max = this.origin = origin;
    }

    public FloatProperty(float origin, float max)
    {
        this.value = this.origin = origin;
        this.max = max;
    }

    public void Reset()
    {
        this.value = this.origin;
    }
}
