using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

[RequireComponent(typeof(Toggle))]
public class SetBoolByToggle : MonoBehaviour
{
    private Toggle toggle;

    private void Awake()
    {
        this.toggle = this.GetComponentInChildren<Toggle>();
    }

    public void OnValueChanged(UnityEngine.Object obj)
    {
        try
        {
            var value = this.toggle.isOn;
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
