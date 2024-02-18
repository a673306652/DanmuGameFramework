using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OccupyBar : MonoBehaviour
{
    public class Data
    {
        public int index = -1;
        public Color color;
        public float speed;
    }

    private Data current = new Data();
    private Data last = new Data();

    public float value;
    [SerializeField]
    private float total;
    private Image bar;
    public Action<int> onCompleted;

    private void Awake()
    {
        this.bar = this.transform.Find<Image>("Bar");
    }

    void Update()
    {
        if (this.onCompleted == null) return;
        if (this.last.index != this.current.index)
        {
            if (this.value <= 0)
            {
                this.last = this.current;
            }
            else
            {
                this.value -= this.current.speed * Time.deltaTime;
                this.bar.color = this.last.color;
            }
        }
        else
        {
            this.value += this.current.speed * Time.deltaTime;
            this.bar.color = this.current.color;
            if (this.value >= this.total)
            {
                this.onCompleted?.Invoke(this.current.index);
                this.onCompleted = null;
            }
        }
        this.bar.fillAmount = this.value / this.total;
    }

    public void SetTotal(int total)
    {
        this.total = total;
    }

    public void Occupy(int index, float speed, Color color)
    {
        this.current.index = index;
        this.current.speed = speed;
        this.current.color = color;
    }

    public void Clear()
    {
        this.last.index = -1;
        this.value = this.total = 0;
        this.onCompleted = null;
        this.bar.fillAmount = 0;
    }
}
