using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Daan;

public class ThrowableObject : AutoDespawnObject
{
    [HideInInspector]
    public Rigidbody rigid;
    public delegate bool OnTrigger(Collider collider);

    public OnTrigger onTrigger;

    public override void Init()
    {
        base.Init();
        this.rigid = this.GetComponentInChildren<Rigidbody>();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        this.rigid.velocity = Vector3.zero;
        this.onTrigger = null;
        this.transform.localScale = Vector3.one;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (this.onTrigger != null && this.onTrigger.Invoke(other))
        {
            ResourceManager.Instance.Despawn(this);
        }
    }
}
