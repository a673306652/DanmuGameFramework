using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
using DanMuGame;
using System;

[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// 默认顺序为 上左后右前下
/// </summary>
public class Dice : PoolObject
{
    public delegate void Event(Dice dice, int face);
    public List<string> targetLayer;
    public List<GameObject> targetObj;


    public bool IsLand(GameObject obj)
    {
        return this.targetObj.Contains(obj) || this.targetLayer.Contains(LayerMask.LayerToName(obj.layer));
    }

    [HideInInspector]
    public new Rigidbody rigidbody;

    private List<GameObject> stayingList = new List<GameObject>();

    public Event endCall;
    public Event startCall;
    private void Awake()
    {
        this.Init();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        this.enabled = true;
        this.rigidbody.velocity = Vector3.zero;
        this.endCall = null;
        this.stayingList.Clear();
    }

    public override void Init()
    {
        base.Init();
        this.rigidbody = this.GetComponentInChildren<Rigidbody>();
    }

    private void Update()
    {
        if (this.endCall != null)
        {
            if (this.IsStable())
            {
                var result = this.GetDiceResult();
                if (this.gameObject.activeInHierarchy)
                {
                    this.endCall?.Invoke(this, result);
                    this.endCall = null;
                }
                this.enabled = false;
            }
            this.stayingList.Clear();
        }
    }

    public bool IsStable()
    {
        var isLand = false;
        foreach (var item in this.stayingList)
        {
            isLand |= this.IsLand(item);
            if (isLand) break;
        }
        return isLand && this.rigidbody.velocity.sqrMagnitude <= 0.1F && this.rigidbody.angularVelocity.sqrMagnitude <= 0.1F;
    }

    /// <summary>
    /// 获取骰子的哪个面朝上
    /// </summary>
    /// <returns></returns>
    public int GetDiceResult()
    {
        var min = float.MaxValue;
        var index = 0;
        var list = new List<Vector3>() { this.transform.up, -this.transform.right, -this.transform.forward, this.transform.right, this.transform.forward, -this.transform.up };
        for (int i = 0; i < list.Count; i++)
        {
            var angle = Vector3.Angle(list[i], Vector3.up);
            if (angle < min)
            {
                min = angle;
                index = i;
            }
        }
        return index;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!this.gameObject.activeInHierarchy || !collision.gameObject.activeInHierarchy) return;
        this.stayingList.Add(collision.gameObject);
    }
}
