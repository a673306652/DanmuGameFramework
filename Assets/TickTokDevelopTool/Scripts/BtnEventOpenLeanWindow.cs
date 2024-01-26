using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;

public class BtnEventOpenLeanWindow : MonoBehaviour
{
    [SerializeField] private LeanWindow targetWindow;

    private void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            targetWindow.TurnOn();
        });
    }
}
