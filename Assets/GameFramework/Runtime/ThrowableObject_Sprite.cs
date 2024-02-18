using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Daan;

public class ThrowableObject_Sprite : ThrowableObject
{
    public SpriteRenderer spriteRenderer;

    public void SetSprite(Sprite sprite)
    {
        this.spriteRenderer.sprite = sprite;
        var size = new Vector2(sprite.texture.width, sprite.texture.height);
        var rate = 100 / size.y;
        this.spriteRenderer.transform.localScale = Vector3.one * rate;
    }
}
