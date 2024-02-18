using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static partial class MonoBehaviourExtension
{
    public static RectTransform GetRectTransform(this MonoBehaviour obj)
    {
        return obj.GetComponent<RectTransform>();
    }

}

