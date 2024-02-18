using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;

public class TextMeshBestFit : MonoBehaviour
{
    public Vector2 charSize;
    public Vector2 charCount;
    [HideInInspector]
    public TextMesh tm;

    private void Awake()
    {
        this.tm = this.GetComponent<TextMesh>();
    }

    private void Update()
    {
        var count = Mathf.Clamp(this.tm.text.Length, this.charCount.x, this.charCount.y);

        var rate = (count - this.charCount.x) / (this.charCount.y - this.charCount.x);

        this.tm.characterSize = Mathf.Lerp(this.charSize.x, this.charSize.y, rate);
    }
}
