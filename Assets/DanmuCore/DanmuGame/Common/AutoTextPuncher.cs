using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class AutoTextPuncher : MonoBehaviour
{
    public float time;
    public float punchForce;
    private Tweener tweener;
    private Text text;
    private string lastTxt;

    private void Awake()
    {
        this.text = this.GetComponent<Text>();
    }

    void Update()
    {
        if (this.lastTxt != null && this.lastTxt != this.text.text)
        {
            if (null == this.tweener)
            {
                this.tweener = this.text.transform.DOPunchScale(Vector3.one * this.punchForce, this.time, 1, 1).SetEase(Ease.Linear).OnComplete(() => this.tweener = null);
            }
        }

        this.lastTxt = this.text.text;
    }
}
