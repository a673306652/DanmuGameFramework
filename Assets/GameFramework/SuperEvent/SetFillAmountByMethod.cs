using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using DanMuGame;
[RequireComponent(typeof(Image))]

public class SetFillAmountByMethod : MonoBehaviour
{
    public UnityEngine.Object target;
    private Image img;

    private void Awake()
    {
        this.img = this.GetComponentInChildren<Image>();
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
                    this.img.fillAmount = (float)result;
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
