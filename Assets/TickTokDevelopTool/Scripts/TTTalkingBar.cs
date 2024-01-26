using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTTalkingBar : MonoBehaviour
{
    public Sprite[] SendSprites;
    public InputField inputer;
    public Image BtnSend;

    private void Update()
    {
        if ( string.IsNullOrEmpty(inputer.text))
        {
            BtnSend.sprite = SendSprites[0];
        }
        else
        {
            BtnSend.sprite = SendSprites[1];
        }
    }
}
