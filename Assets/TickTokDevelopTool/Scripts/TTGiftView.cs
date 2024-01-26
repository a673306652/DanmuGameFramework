using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTGiftView : MonoBehaviour
{

    public TTGiftItem[] items;
    public static TTGiftView Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Select(int id)
    {
        for (var i = 0; i < items.Length; i++)
        {
            items[i].CancelSelect();
        }
        items[id].Select();
    }
}
