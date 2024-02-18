using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Daan;
using System;
public class Bubble_Text : Bubble
{
    public string value;
    private Text txt;
    public override void Init()
    {

        base.Init();
        this.txt = this.transform.Find("背景/文字").GetComponent<Text>();
    }

    protected override Vector2 GetSize()
    {
        var w = this.txt.preferredWidth + 20;
        var h = this.txt.preferredHeight + 20;
        return new Vector2(w, h);
    }

    public void SetContent(string str, int fontSize = 30)
    {
        this.value = str;
        this.txt.text = this.value;
        this.txt.fontSize = fontSize;
    }
}
