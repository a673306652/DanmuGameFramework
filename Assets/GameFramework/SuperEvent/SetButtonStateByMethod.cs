using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using DanMuGame;
[RequireComponent(typeof(Button))]

public class SetButtonStateByMethod : MonoBehaviour
{
    public UnityEngine.Object target;
    private Button btn;

    private void Awake()
    {
        this.btn = this.GetComponentInChildren<Button>();
    }

    private void Update()
    {
        try
        {
            var type = this.target.GetType();
            foreach (var item in type.GetMethods())
            {
                if (item.Name == this.name)
                {
                    var result = item.Invoke(this.target, null);
                    this.btn.interactable = (bool)result;
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

}
