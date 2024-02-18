using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalkBubble : MonoBehaviour
{
    public Queue<string> bubbleQueue;

    public Animator anim;
    public TextMeshProUGUI tmp;
    
    private void Awake()
    {
        bubbleQueue = new Queue<string>();
        anim = GetComponentInChildren<Animator>();
        tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void RequestBubble(string content)
    {
        bubbleQueue.Enqueue(content);
    }

    public float tick;
    
    private void Update()
    {
        if (bubbleQueue.Count>0&& tick<=0)
        {
            var bb= bubbleQueue.Dequeue();
            tmp.text = bb;
            anim.Play("Open");
            tick = 3f;
        }

        if (tick>0)
        {
            tick -= Time.deltaTime;
            if (tick<=0.5f)
            {
                anim.Play("Close");
            }
        }
    }
}
