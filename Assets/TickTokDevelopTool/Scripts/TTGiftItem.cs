using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTGiftItem : MonoBehaviour
{

    public GameObject SelectContent;
    public Button BtnSend;
    public Button BtnSelect;
    public int id;
    public string GiftID;
    public string GiftName;
    public Sprite GiftSprite;

    private void Awake()
    {
        BtnSelect.onClick.AddListener(OnClick);
        BtnSend.onClick.AddListener(Send);
    }

    private void Start()
    {
        CancelSelect();
    }

    public void OnClick()
    {
        TTGiftView.Instance.Select(id);
    }

    public void Send()
    {
        TTTestTool.Instance.TTGift(GiftID,GiftName,this);
    }
    public void Select()
    {
        SelectContent.SetActive(true);
    }

    public void CancelSelect()
    {
        SelectContent.SetActive(false);
    }


}
