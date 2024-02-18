using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettlementItem : MonoBehaviour
{
    protected Text decsTxt;

    protected Text nameTxt;

    protected Text valueTxt;

    private void Awake()
    {
        this.decsTxt = this.transform.Find("desc").GetComponent<Text>();
        this.nameTxt = this.transform.Find("name").GetComponent<Text>();
        this.valueTxt = this.transform.Find("value").GetComponent<Text>();
    }

    public void SetUp(string name ,string desc , float value)
    {
        this.nameTxt.text = name;
        this.decsTxt.text = desc;
        this.valueTxt.text = value + "";
    }

}
