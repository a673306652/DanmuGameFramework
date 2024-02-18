using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : HisaoMono
{
    public GameObject[] Effects;

    public void Play(int id)
    {
        var ps = Effects[id].GetComponentInChildren<ParticleSystem>();
        if (null != ps)
        {
            ps.Play();
        }
    }
    
    public void Stop(int id)
    {
        var ps = Effects[id].GetComponentInChildren<ParticleSystem>();
        if (null != ps)
        {
            ps.Stop();
        }
    }
}

public struct EffectID
{
    public const int HealingEffect = 0;
}