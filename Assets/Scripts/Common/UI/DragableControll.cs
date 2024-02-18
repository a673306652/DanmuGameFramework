using System;
using System.Collections;
using System.Collections.Generic;
using Hisao;
using UnityEngine;
using UnityEngine.Events;

public class DragableControll : HisaoMono
{

    public static DragableControll Instance;


    private void Awake()
    {
        Instance = this;
    }

    public Transform dragedObject;

    public delegate bool OnDragComple();

    public OnDragComple onComplete;

    private Vector3 tempV3;
    private Vector3 tempMouse;

    public void UpdateDragableUI(float delta)
    {
        if (null != dragedObject)
        {
            var dir =
                (HisaoTransform.Screen2WorldPos(Input.mousePosition, UIManager.Instance.OverlayCam, 0) - tempMouse)
                .normalized.xy();

            dragedObject.transform.position = tempV3 + dir * Vector3.Distance(
                HisaoTransform.Screen2WorldPos(Input.mousePosition, UIManager.Instance.OverlayCam, 0), tempMouse);
        }
    }

    public void DragTarget(Transform obj, OnDragComple oc)
    {
        if (dragedObject == obj)
        {
            return;
        }

        tempV3 = obj.position;
        tempMouse = HisaoTransform.Screen2WorldPos(Input.mousePosition, UIManager.Instance.OverlayCam, 0);
        dragedObject = obj;
        onComplete = oc;

        GLOBAL_TIME = 0.1f;
    }

    public void ForceFailed()
    {
        GLOBAL_TIME = 1f;


        if (null != dragedObject)
        {
            dragedObject.transform.position = tempV3;
        }


        dragedObject = null;
    }

    public void DragComplete(UnityAction<Transform> oc)
    {
        GLOBAL_TIME = 1f;
        var succed = onComplete.Invoke();
        if (!succed)
        {
            if (null != dragedObject)
            {
                dragedObject.transform.position = tempV3;
            }
        }
        else
        {
            oc.Invoke(dragedObject);
            if (null != dragedObject)
            {
                dragedObject.transform.position = tempV3;
            }
        }

        dragedObject = null;
    }
}