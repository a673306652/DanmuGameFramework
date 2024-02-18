using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

[RequireComponent(typeof(InputField))]
public class InputFieldEvent : MonoBehaviour
{
    private InputField input;

    private void Awake()
    {
        this.input = this.GetComponentInChildren<InputField>();
    }

    public void OnValueChanged(UnityEngine.Object obj)
    {
        try
        {
            var value = this.input.text;
            var type = obj.GetFieldValue(this.name).GetType();
            var fValue = Convert.ChangeType(value, type);
            obj.SetFieldValue(this.name, fValue);
        }
        catch (Exception e)
        {
            return;
        }

    }
}
