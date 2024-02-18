 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "BezierShaper/BezierPreset")]   
public class BezierShaperPreset : ScriptableObject 
{
    public float StartSpeed;
    public float StartDuration;
    public float FlySpeed;
}