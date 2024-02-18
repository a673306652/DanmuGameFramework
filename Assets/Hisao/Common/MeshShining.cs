using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshShining : ShiningEffect
{
    [SerializeField] private RenderMode _RenderMode;
    public GameObject target;
    private enum RenderMode
    {
        Lit,Unlit,Toon
    }
    protected override void Init()
    {
        
        base.Init();
        // string key = "";
        // switch (_RenderMode)
        // {
        //     case RenderMode.Lit:
        //         key = "Hisao/Hisao_SimpleLit";
        //         break;
        //     case RenderMode.Unlit:
        //         key = "Hisao/Hisao_SimpleUnlit";
        //         break;
        //     case RenderMode.Toon:
        //         key = "Hisao/XG_Toon";
        //         break;
        // }
        
        // var shader = Shader.Find(key);
        // var tempMat = GetComponent<Renderer>().material;
        // GetComponent<Renderer>().material = new Material(shader);

        var r = GetComponentsInChildren<Renderer>();
        shiningMat = new Material[r.Length];
        
        for (var i = 0; i < r.Length; i++)
        {
            shiningMat[i] = r[i].material;
            shiningMat[i].SetFloat("_Shining", 1f);
        }
       
        // shiningMat.CopyPropertiesFromMaterial(tempMat);
     
    }
}
