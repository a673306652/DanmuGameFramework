using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class HpBar : MonoBehaviour
{
    public Image bg;
    public Image bar;

    public Color min;
    public Color max;

    private Sequence fadeSequence;

    private Color originBGColor;

    private float fadeTimer = 0;
    private void Awake()
    {
        this.originBGColor = this.bg.color;
    }
    public void SetValue(float value)
    {
        this.bar.fillAmount = value;
    }
    public void Show()
    {
        this.fadeSequence?.Kill();
        this.bg.color = this.originBGColor;
        this.bar.color = this.bar.color.NewA(1);
        this.fadeTimer = 2F;
    }
    public void FadeOut()
    {
        this.fadeSequence?.Kill(true);
        this.fadeSequence = DOTween.Sequence();
        this.fadeSequence.Join(this.bg.DOFade(0, 0.5F));
        this.fadeSequence.Join(this.bar.DOFade(0, 0.5F));
    }

    public void Close()
    {
        this.FadeOut();
        this.fadeSequence?.Kill(true);
    }

    private void Update()
    {
        if (this.fadeTimer > 0)
        {
            this.fadeTimer -= Time.deltaTime;
            if (this.fadeTimer <= 0)
            {
                this.FadeOut();
            }
        }
        if (this.bar.color.a > 0)
        {
            var fill = this.bar.fillAmount;
            var color = Color.Lerp(this.min, this.max, fill);
            this.bar.color = color.NewA(this.bar.color.a);
        }
        if (Input.GetKeyDown("a"))
        {
            this.FadeOut();
        }
        if (Input.GetKeyDown("s"))
        {
            this.Show();
        }
    }
}
