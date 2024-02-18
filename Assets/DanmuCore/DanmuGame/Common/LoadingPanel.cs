using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Daan;

public class LoadingPanel : Manager<LoadingPanel>
{
    [HideInInspector]
    public Animator anim;

    private void Awake()
    {
        this.gameObject.SetActive(false);
        this.anim = this.GetComponent<Animator>();
    }


    public void Show(Action action)
    {
        this.gameObject.SetActive(true);
        this.anim.Play("Show", 0, 0);
        Timer.Register(0.5F, action);
    }

    public void Close(Action action)
    {
        this.anim.Play("Close", 0, 0);
        Timer.Register(0.5F, ()=> 
        {
            this.gameObject.SetActive(false);
            action?.Invoke();
        });
    }
}
