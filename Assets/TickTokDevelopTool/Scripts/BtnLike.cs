using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnLike : MonoBehaviour
{
    private void Awake()
    {
        var c = GetComponent<Button>();
        c.onClick.AddListener(Like);
    }

    private void Like()
    {
        TTTestTool.Instance.TTLike();
    }
}
