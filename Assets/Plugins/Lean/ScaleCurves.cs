using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/ScaleCurves")]
[Serializable]
public class ScaleCurves : ScriptableObject
{

    [BoxGroup("Scale Curve")]
    [SerializeField]
    public AnimationCurve ScaleX = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [BoxGroup("Scale Curve")]
    [SerializeField]
    public AnimationCurve ScaleY = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [BoxGroup("Scale Curve")]
    [SerializeField]
    public AnimationCurve ScaleZ = AnimationCurve.Linear(0f, 0f, 1f, 1f);
   

}