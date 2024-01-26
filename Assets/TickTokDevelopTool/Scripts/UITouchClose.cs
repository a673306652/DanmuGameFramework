using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITouchClose : HisaoMono 
{
    private LeanWindow lw;

    private static int GUID;

    private int MyGUID;

    private void Awake()
    {
        lw = GetComponent<LeanWindow>();
        var clicker = GetComponentsInChildren<Image>();
        GUID++;
        MyGUID = GUID;
        for (var i = 0; i < clicker.Length; i++)
        {
            var c = clicker[i].gameObject.AddComponent<TouchingBody>();
            c.GUID = MyGUID;
        }
        
        var clicker2 = GetComponentsInChildren<Text>();
        
        for (var i = 0; i < clicker2.Length; i++)
        {
            var c = clicker2[i].gameObject.AddComponent<TouchingBody>();
            c.GUID = MyGUID;
        }
       
    }

    private void Start()
    {
        TTClickEvent.Instance.AddListenerEvent(OnPointerClick);
    }


    public void OnPointerClick(GameObject eventObj)
    {
 
        if (null == eventObj)
        {
            if (lw.On == true)
            {
                lw.On = false;
            }
            return;
        }
        if (eventObj.GetComponent<TouchingBody>())
        {
            if (eventObj.GetComponent<TouchingBody>().GUID == MyGUID)
            {
          
                return;
            }
            else
            {
                if (lw.On == true)
                {
                    lw.On = false;
                }
                return;
            }
        }

        if (eventObj != gameObject)
        {
            if (lw.On == true)
            {
                lw.On = false;
            }
        }
    }
}

public class TouchingBody : HisaoMono
{
    public int GUID;
}