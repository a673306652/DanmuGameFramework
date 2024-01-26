using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnUserPreset : MonoBehaviour
{
    public string UID;
    public string nickName;

    public void Awake()
    {
        var c = GetComponent<Button>();
        c.onClick.AddListener(ChangePreset);
    }

    private void ChangePreset()
    {
        TTTestTool.Instance.uidText.text = UID;
        TTTestTool.Instance.nickNameText.text = nickName;
        TTTestTool.Instance._uid = UID;
        TTTestTool.Instance._nickName = nickName;
    }
}
