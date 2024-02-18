using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEvent : MonoBehaviour
{
    public string eventName;
    [HideInInspector]
    public new Collider collider;
    public CollisionListener listener;

    protected virtual void Awake()
    {
        if (this.listener == null)
        {
            this.listener = this.GetComponentInParent<CollisionListener>();
        }
        this.collider = this.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.listener == null || !this.gameObject.activeInHierarchy || !other.gameObject.activeInHierarchy) return;
        this.listener.Excute(this.eventName, this, other, 0);
    }

    private void OnTriggerStay(Collider other)
    {
        if (this.listener == null || !this.gameObject.activeInHierarchy || !other.gameObject.activeInHierarchy) return;
        this.listener.Excute(this.eventName, this, other, 1);
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.listener == null || !this.gameObject.activeInHierarchy || !other.gameObject.activeInHierarchy) return;
        this.listener.Excute(this.eventName, this, other, 2);
    }


    private void OnCollisionEnter(Collision other)
    {
        if (this.listener == null || !this.gameObject.activeInHierarchy || !other.gameObject.activeInHierarchy) return;
        this.listener.Excute(this.eventName, this, other, 0);
    }

    private void OnCollisionStay(Collision other)
    {
        if (this.listener == null || !this.gameObject.activeInHierarchy || !other.gameObject.activeInHierarchy) return;
        this.listener.Excute(this.eventName, this, other, 1);
    }

    private void OnCollisionExit(Collision other)
    {
        if (this.listener == null || !this.gameObject.activeInHierarchy || !other.gameObject.activeInHierarchy) return;
        this.listener.Excute(this.eventName, this, other, 2);
    }
}
