using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BtnPuncher : MonoBehaviour
{
    private Vector3 originSize;
    private void Awake()
    {
        var c = GetComponent<Button>();
        c.onClick.AddListener(Punch);
        originSize = transform.localScale;
    }

    private void Punch()
    {
        transform.localScale = originSize;
        transform.DOPunchScale(Vector3.one * 0.2f, 0.5f);
    }
}
