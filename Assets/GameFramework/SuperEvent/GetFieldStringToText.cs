using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using DanMuGame;
[RequireComponent(typeof(Text))]

public class GetFieldStringToText : MonoBehaviour
{
    public UnityEngine.Object target;
    private Text text;
    private void Awake()
    {
        this.text = this.GetComponentInChildren<Text>();

    }
    private void Update()
    {
        try
        {
            var result = this.target.GetFieldValue<string>(this.name);
            this.text.text = result;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

}
