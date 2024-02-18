using System;
using System.Linq;
using UnityEngine;

public static class AnimatorExt
{
    public static AnimationClip GetAnimationClip(this Animator animator, string name)
    {
        if (null == animator)
        {
            Debug.LogError("Animator could not be null!");
            return null;
        }
        RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
        if (null == runtimeAnimatorController)
        {
            Debug.LogError($"Animator 【{animator.name}】 does not have a mounted controller!");
            return null;
        }
        AnimationClip[] clips = runtimeAnimatorController.animationClips;
        AnimationClip[] varclip = Array.FindAll(clips, v => v.name == name);
        if (null == varclip || varclip.Length == 0)
        {
            throw new InvalidOperationException($"Animator 【{animator.name}】 does not have the animation clip 【{name}】!");
        }
        if (varclip.Length >= 2)
        {
            Debug.LogWarning($"Animator 【{animator.name}】 has 【{varclip.Length}】 clips with name 【{name}】.");
        }
        return varclip[0];
    }

    public static AnimationEvent GetEventAtFrame(this AnimationClip clip, int frame)
    {
        AnimationEvent[] events = clip.events;
        float time = frame / clip.frameRate;
        return Array.Find(events, v => Mathf.Approximately(v.time, time));
    }
}