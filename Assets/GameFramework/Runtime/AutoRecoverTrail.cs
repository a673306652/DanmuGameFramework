using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRecoverTrail : MonoBehaviour
{
    [HideInInspector]
    public TrailRenderer trail;
    private void Awake()
    {
        this.trail = this.GetComponentInParent<TrailRenderer>();
    }

    private void OnDisable()
    {
        this.trail.Clear();
    }
}
