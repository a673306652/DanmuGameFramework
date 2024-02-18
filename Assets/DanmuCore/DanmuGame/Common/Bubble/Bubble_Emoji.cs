using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Daan;
using System;
public class Bubble_Emoji : Bubble
{
    private Image img;

    private float h;
    private float w;

    public float max = 250F;
    public override void Init()
    {
        base.Init();
        this.img = this.transform.Find<Image>("背景/图片");
    }

    protected override Vector2 GetSize()
    {
        var w = this.w + 5;
        var h = this.h + 5;
        return new Vector2(w, h);
    }

    public void SetSprite(Sprite sprite)
    {
        this.img.sprite = sprite;

        var w = sprite.texture.width;
        var h = sprite.texture.height;
        var rate = 0f;
        if (w > h)
        {
            rate = w / this.max;
            this.w = this.max;
            this.h = h / rate;
        }
        else
        {
            rate = h / this.max;
            this.h = this.max;
            this.w = w / rate;
        }
        this.img.rectTransform.sizeDelta = new Vector2(this.w, this.h);

    }
}
