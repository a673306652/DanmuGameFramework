using Daan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeBar : Daan.PoolObject
{
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Text text;
    private void Awake()
    {
        this.anim = this.GetComponentInChildren<Animator>();
        this.text = this.GetComponentInChildren<Text>();
        this.gameObject.SetActive(false);
    }

    public void Show(string content)
    {
        this.text.text = content;
        this.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
    }

}
