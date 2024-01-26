using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFade : MonoBehaviour
{
    private Material[] m;
    public float lifeTime;
    private void Awake()
    {
        m = GetComponent<Renderer>().materials;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        for (int i = 0; i < m.Length; i++)
        {
            m[i].SetFloat("_Alpha",lifeTime/1);
        }
    }
}
