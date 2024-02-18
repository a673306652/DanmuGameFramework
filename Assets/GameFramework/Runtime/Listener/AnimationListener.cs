using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationListener : MonoBehaviour
{
    public delegate void AnimationDelegate();

    private Dictionary<string, AnimationDelegate> dict = new Dictionary<string, AnimationDelegate>();

    public void Register(string eventName, AnimationDelegate d)
    {
        if (!this.dict.ContainsKey(eventName)) this.dict.Add(eventName, null);
        this.dict[eventName] += d;
    }

    public void AnimationEvent(string eventName)
    {
        if (!this.dict.ContainsKey(eventName)) return;
        this.dict[eventName].Invoke();
    }
}
