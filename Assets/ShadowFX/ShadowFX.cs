using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFX : MonoBehaviour
{
    public Transform root;

    public Material vfxMat;

    public float shadowPerTime;
    
    public float time;
    
    public Renderer smr;

    private bool onPlay;
    [SerializeField]private float timeTick;
    [SerializeField]private float timeTick2;
    private void Update()
    {
        if (timeTick>0)
        {
            timeTick -= Time.deltaTime;
            timeTick2 += Time.deltaTime;
            if (timeTick2 > shadowPerTime)
            {
                timeTick2 = 0;
                CreatOnce();
            }
        }
    }

    private void CreatOnce()
    {
        var a = new GameObject();

        a.transform.transform.position = root.position;
        a.transform.forward = root.right;
        a.AddComponent<MeshFilter>();
        a.AddComponent<MeshRenderer>();
        if (smr.GetType().Equals(typeof(SkinnedMeshRenderer)))
        {
            var skin = smr.GetComponent<SkinnedMeshRenderer>();
            skin.BakeMesh(a.GetComponent<MeshFilter>().mesh);
        }
        else
        {
            a.GetComponent<MeshFilter>().mesh = smr.GetComponent<MeshFilter>().mesh;
        }
     
        a.GetComponent<MeshRenderer>().materials = new Material[2] {vfxMat, vfxMat
    };
        a.AddComponent<ShadowFade>();
        a.GetComponent<ShadowFade>().lifeTime = 0.4f;
    Destroy(a,0.4f);
    }
    public void VFXGo()
    {
        onPlay = true;
        timeTick = time;
        timeTick2 = shadowPerTime;
    }

}
