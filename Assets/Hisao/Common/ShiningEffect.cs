using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiningEffect : MonoBehaviour
{
    
    protected Material[] shiningMat;
    protected float _shiningValue;

    private void OnEnable()
    { 
        Init();
    }

    protected virtual void Init()
    {
         
    }
    
    [ContextMenu("tst")]
    public void Shining()
    {
        _shiningValue = 1;
    }
    
    void Update()
    {
        try
        {
            if (_shiningValue>0)
            {
                _shiningValue -= Time.deltaTime*3;
            }
            else
            {
                _shiningValue = 0;
            }

            for (var i = 0; i < shiningMat.Length; i++)
            {
                shiningMat[i].SetFloat("_Shining", _shiningValue);
            }
           
        }
        catch
        {
            
        }
    }
    
    
}