using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveCaster : MonoBehaviour
{
    public float interactiveRadios;
    public LayerMask interactiveLayerMask;
    private float iCD;
    private float timeTick;

    public GameObject InteractiveUI;
    private void Update()
    {
        
        
        var a = Physics.OverlapSphere(transform.position, interactiveRadios, interactiveLayerMask);

        if (null!=InteractiveUI)
        {
                InteractiveUI.SetActive(a.Length>0);
        }
    
    }

    public void tryInteractive()
    {
        if (iCD>0)return;
        
        var a = Physics.OverlapSphere(transform.position, interactiveRadios, interactiveLayerMask);

        foreach (var iTarget in a)
        {
            if (iTarget.GetComponent<InteractiveBase>())
            {
                iTarget.GetComponent<InteractiveBase>().OnStart();
                
            }
        }

        iCD = 1f;
        StartCoroutine(delayCD());
    }

    IEnumerator delayCD()
    {
        yield return new WaitForSeconds(0.5f);
        iCD = 0;
    }
    private IEnumerator tickEvent(InteractiveBase ib)
    {
        var a = 0;
        while (a==0)
        {
         ib.OnUpdate();
         yield return new WaitForEndOfFrame();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,interactiveRadios);
    }
#endif
}
