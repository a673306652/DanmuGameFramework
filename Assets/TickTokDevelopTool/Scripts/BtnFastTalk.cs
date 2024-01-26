using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnFastTalk : MonoBehaviour
{
    public string content;
    private void Start()
    {
        var c = GetComponent<Button>();
        c.onClick.AddListener(FTalk);
    }

    private void FTalk()
    {
        TTTestTool.Instance.TTTalk(content);
    }
}
