using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
using UnityEngine.UI;

public class BattleText : AutoDespawnObject
{
    private RectTransform root;
    private Text text;
    private Animator anim;

    public override void Init()
    {
        base.Init();
        this.anim = this.GetComponentInChildren<Animator>();
        this.text = this.GetComponentInChildren<Text>();
        this.root = this.GetComponent<RectTransform>();
    }

    public void Show(string content, Vector3 worldPos, Transform parent = null)
    {
        if (!parent) parent = ResourceManager.Instance.canvas.transform;
        this.transform.SetParent(parent);
        this.root.anchoredPosition = Camera.main.WorldToCanvasPoint(ResourceManager.Instance.canvas.GetRectTransform(), worldPos);
        this.text.text = content;
        this.anim.Play("Show", 0, 0);
    }
}
