using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
[RequireComponent(typeof(Text))]

public class GetFieldToText : MonoBehaviour
{
    public UnityEngine.Object target;
    private Text text;
    public TypeCode type;
    private void Awake()
    {
        this.text = this.GetComponentInChildren<Text>();
    }


    private void Update()
    {
        try
        {
            var result = this.target.GetFieldValue(this.name);
            var finalResult = Convert.ChangeType(result, this.type);
            this.text.text = finalResult.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

}
