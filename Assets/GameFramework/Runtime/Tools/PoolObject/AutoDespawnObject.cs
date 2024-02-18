using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
public class AutoDespawnObject : PoolObject
{
    public float autoDespawnTime;
    public float autoDespawnTimer;


    public override void Init()
    {
        base.Init();
        this.autoDespawnTimer = this.autoDespawnTime;
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        this.autoDespawnTimer = this.autoDespawnTime;
    }

    protected virtual void Update()
    {
        if (this.autoDespawnTimer > 0)
        {
            this.autoDespawnTimer -=  Time.deltaTime;
            if (this.autoDespawnTimer <= 0)
            {
                this.OnTimeOut();
            }
        }
    }

    public virtual void OnTimeOut()
    {
        Daan.ResourceManager.Instance.Despawn(this);
    }
}
