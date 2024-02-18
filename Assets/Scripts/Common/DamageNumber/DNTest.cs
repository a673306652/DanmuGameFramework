using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using NaughtyAttributes;
using UnityEngine;

public class DNTest : MonoBehaviour
{
    public static DNTest Instance;
    

    private void Awake()
    {
        Instance = this;
    }

    public DamageNumber dn;
    public DamageNumber dn2;
    public DamageNumber dn3;
    
    public void t(Vector3 pos,string content)
    {
        dn.Spawn(pos,content);
    }
    
    public void t2(Vector3 pos,string content)
    {
        dn2.Spawn(pos,content);
    }

    public void t3(Vector3 pos, string content)
    {
        dn3.Spawn(pos, content);
    }
}
