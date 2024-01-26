using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbMaster : MonoBehaviour
{
    public float capHeigh;
    public float climbRadios;
    public Vector3 tilling;

    public LayerMask climbLayer;

    private bool canClimb;
    // Update is called once per frame
    void Update()
    {
        var climbBox = Physics.OverlapCapsule(transform.position + tilling, transform.position+new Vector3(0,capHeigh,0) +tilling,climbRadios, climbLayer);

        if (climbBox.Length>0)
        {
            canClimb = true;
        }
        else
        {
            canClimb = false;
        }
    }

    public bool GetCanClimb()
    {
        return canClimb;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawSphere(transform.position + tilling,climbRadios);
        Gizmos.DrawSphere(transform.position+new Vector3(0,capHeigh,0) +tilling ,climbRadios);
    }
#endif
}
