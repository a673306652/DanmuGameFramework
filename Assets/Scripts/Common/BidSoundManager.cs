using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BidSoundManager : MonoBehaviour
{
    public static BidSoundManager instance;
    public GameObject templateBidSoundAgent;
    public List<BidSoundAgent> agent = new List<BidSoundAgent>();
    public AudioClip[] clips;

    private const int MAIN_SOUND_MANAGER_COUNT = 20;
    private void Awake()
    {
        instance = this;
        for (int i = 0; i < 10; i++)
        {
            agent.Add(RegisterAgent());
        }
    }

    public BidSoundAgent Play(AudioClip clip,bool ignoreSpeed =false)
    {
        int index = Array.IndexOf(clips, clip) + MAIN_SOUND_MANAGER_COUNT;

        BidSoundAgent b = null;
        for (var i = 0; i < agent.Count; i++)
        {
            if (!agent[i].onPlay)
            {
                b = agent[i];
                break;
            }
        }
        if (null == b)
        {
            b = RegisterAgent();
            agent.Add(b);
        }
        b.Play(clip,ignoreSpeed);
        return b;
    }

    public BidSoundAgent RegisterAgent()
    {
        var a = Instantiate(templateBidSoundAgent, transform);
        return a.GetComponent<BidSoundAgent>();
    }

}
