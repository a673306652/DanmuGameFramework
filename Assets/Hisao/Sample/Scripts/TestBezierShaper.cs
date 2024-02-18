using System.Collections;
using System.Collections.Generic;
using Hisao;
using UnityEngine;

public class TestBezierShaper : MonoBehaviour
{
    public Transform shaper;

    private List<ShapeTask> ta = new List<ShapeTask>();
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        // var shapeRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // var shapePos = Vector3.zero;
        // RaycastHit rh = new RaycastHit();
        // if (Physics.Raycast(shapeRay,out rh))
        // {
        //     shapePos = rh.point;
        // }
        // if (Input.GetKey(KeyCode.Mouse0))
        // {
        //     var shaper = PerfectBezierShaper.instance;
        //   
        //
        //     var x = shaper.ShapeObj2D(shaper.GetBSPreset(BezierShaperResourcesUrl.StarPreset),
        //         shaper.GetBSResources(BezierShaperResourcesUrl.StarBullet),
        //         shaper.GetBSResources(BezierShaperResourcesUrl.StarExp),shaper.transform.position,shapePos,1,null).WithEndAction(
        //         (item) =>
        //         {
        //             ta.Remove(item);
        //         }).Play();
        //     ta.Add(x);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     if (ta.Count>0)
        //     {
        //         for (var i = 0; i < ta.Count; i++)
        //         {
        //             ta[i].StopAndDetonateAll();
        //         }
        //     }
        //     ta.Clear();
        // }
        //
        // shaper.forward = (shapePos - shaper.position).normalized;
    }
}