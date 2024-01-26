using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnTalk : MonoBehaviour
{
    public InputField inf;
    private void Awake()
    {
        var c = GetComponent<Button>();
        c.onClick.AddListener(Talk);
    }

    private void Talk()
    {
        var x = inf.text;
        if (!string.IsNullOrEmpty(x))
        {
            TTTestTool.Instance.TTTalk(x);
            inf.text = string.Empty;
        }
    }
}
