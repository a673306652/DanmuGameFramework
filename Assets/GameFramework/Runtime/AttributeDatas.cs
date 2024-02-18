using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttributeDatas
{
    /// <summary>
    /// @Daan ，之后不要写死在这个类里，而是从外部传进来
    /// </summary>
    public enum Attribute : int
    {
        血量,
        攻击力,
        防御力,
        技能冷却,
        移动速度,
        撞击力,
        撞击抗性,
    }

    public delegate float Calc(float orgin);

    /// <summary>
    /// 基础值
    /// </summary>
    private Dictionary<Attribute, float> origin = new Dictionary<Attribute, float>();

    /// <summary>
    /// 附加值事件
    /// </summary>
    private Dictionary<Attribute, Dictionary<string, Calc>> bind = new Dictionary<Attribute, Dictionary<string, Calc>>();

    /// <summary>
    /// 最终值，Get直接取这里的值，每次注册，更新，到期都同步一次这个值
    /// </summary>
    private Dictionary<Attribute, float> final = new Dictionary<Attribute, float>();

    public float Get(Attribute type)
    {
        return this.final.TryGet(type);
    }

    public float GetOrigin(Attribute type)
    {
        return this.origin.TryGet(type);
    }

    public void SetOrigin(Attribute type, Func<float, float> action)
    {
        if (!this.origin.ContainsKey(type)) this.origin.Add(type, 0);
        this.origin[type] = action.Invoke(this.origin[type]);
        this.RecalcData(type);
    }

    public void SetOrigin(Attribute type, float value)
    {
        if (!this.origin.ContainsKey(type)) this.origin.Add(type, 0);
        this.origin[type] = value;
        this.RecalcData(type);
    }

    public void SetBind(Attribute type, string key, Calc c)
    {
        if (!this.bind.ContainsKey(type) || !this.bind[type].ContainsKey(key)) return;
        this.bind[type][key] = c;
        this.RecalcData(type);
    }

    public void RegisterBind(Attribute type, string key, Calc c)
    {
        if (!this.bind.ContainsKey(type))
        {
            this.bind.Add(type, new Dictionary<string, Calc>());
        }
        else
        {
            if (this.bind[type].ContainsKey(key))
            {
                Debug.LogError("重复绑定属性");
                return;
            }
        }
        this.bind[type].Add(key, c);
        this.RecalcData(type);
    }

    public void RemoveBind(Attribute type, string key)
    {
        if (this.bind.ContainsKey(type))
        {
            this.bind[type].Remove(key);
            this.RecalcData(type);
        }
    }

    public void Clear()
    {
        this.bind.Clear();
        this.origin.Clear();
        this.final.Clear();
    }

    void RecalcData(Attribute type)
    {
        if (!this.origin.ContainsKey(type)) return;
        var result = 0F;
        if (this.bind.ContainsKey(type))
        {
            foreach (var item in this.bind[type].Values)
            {
                result += item.Invoke(this.origin[type]);
            }
        }
        this.final[type] = this.origin[type] + result;
    }
}

