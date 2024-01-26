using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;

public static class VFX_StaticCurveManager  
{
     public static AnimationCurve GetGameviewInCurve()
     {
          return Resources.Load<ScaleCurves>("StaticCurves/GameView_In").ScaleX;
     }
     
     public static AnimationCurve GetGameviewOutCurve()
     {
          return Resources.Load<ScaleCurves>("StaticCurves/GameView_Out").ScaleX;
     }
     
     public static AnimationCurve GetBVInCurve()
     {
          return Resources.Load<ScaleCurves>("StaticCurves/BusinessView_In").ScaleX;
     }

     public static AnimationCurve GetBVOutCurve()
     {
          return Resources.Load<ScaleCurves>("StaticCurves/BusinessView_Out").ScaleX;
     }
     
       
     public static AnimationCurve GetBVBtnInCurve()
     {
          return Resources.Load<ScaleCurves>("StaticCurves/BusinessView_Btn_In").ScaleX;
     }
     
     public static AnimationCurve GetBVDelayShowCurve()
     {
          return Resources.Load<ScaleCurves>("StaticCurves/BusinessView_DelayShow").ScaleX;
     }
     
     public static AnimationCurve GetBVDelayCloseCurve()
     {
          return Resources.Load<ScaleCurves>("StaticCurves/BusinessView_DelayClose").ScaleX;
     }

     public static AnimationCurve GetBVFillShiningCurve()
     {
          return Resources.Load<ScaleCurves>("StaticCurves/BVFillShining").ScaleX;

     }
}
