using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BidSoundAgent : MonoBehaviour
{
    public AudioSource player;
    public bool onPlay;
    private Coroutine currentPlayingTask;
    private bool _ignoreSpeed;
    public void Play(AudioClip clip,bool ignoreSpeed =false)
    {
        if (onPlay)
        {
            return;
        }
        _ignoreSpeed = ignoreSpeed;
        player.clip = clip;
        player.Play();
        onPlay = true;
        currentPlayingTask = StartCoroutine(PlayAudio());
    }

    public void ForcePlay(AudioClip clip,bool ignoreSpeed =false)
    {
        if (null != currentPlayingTask)
        {
            StopCoroutine(currentPlayingTask);
            currentPlayingTask = null;
        }
        _ignoreSpeed = ignoreSpeed;
        player.Stop();
        player.clip = clip;
        player.Play();
        onPlay = true;
        currentPlayingTask = StartCoroutine(PlayAudio());
    }

    private IEnumerator PlayAudio()
    {
        while (player.isPlaying)
        {
            yield return new WaitForFixedUpdate();
        }

        onPlay = false;
    }

    private void Update()
    {
       
    }

    public void ForceStop()
    {
        player.Stop();
        onPlay = false;
        if (null != currentPlayingTask)
        {
            StopCoroutine(currentPlayingTask);
            currentPlayingTask = null;
        }
    }
}
