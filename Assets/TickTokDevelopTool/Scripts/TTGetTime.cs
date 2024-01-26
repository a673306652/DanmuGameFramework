using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TTGetTime : MonoBehaviour
{
    public Text Timer;
    void Start()
    {
        if (null==Timer)
        {
            Timer = GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (null==Timer)
        {
            Timer = GetComponent<Text>();
        }
        if (null!=Timer)
        {
            Timer.text = FormatTime(System.DateTime.Now.Hour) + ":" +FormatTime( System.DateTime.Now.Minute);
        }
    }

    public string FormatTime(int t)
    {
       var c = t.ToString();
       if (c.Length<=1)
       {
           c = "0" + c;
       }

       return c;
    }
}
