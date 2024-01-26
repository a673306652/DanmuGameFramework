using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveHook : InteractiveBase
{
    public Transform hookPoint;
    public bool isStart;
    public override void OnStart()
    {
 
        simpleHook.instance.hook2Target(hookPoint.position);
        EndInteractive();

    }

    public override void OnUpdate()
    {
        
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (hookPoint==null)return;
        Gizmos.color =Color.magenta;
        Gizmos.DrawSphere(hookPoint.position,1);
    }
#endif
}
