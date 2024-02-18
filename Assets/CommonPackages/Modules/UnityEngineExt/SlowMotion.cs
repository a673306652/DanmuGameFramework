using System;
using UnityEngine;
using System.Collections;
using Modules.Patterns;

public class SlowMotion : MonoSingleton<SlowMotion>
{
    #region Original Scale
    private float oriTimeScale;
    private float oriFixedDeltaTime;
    private bool isScaled;
    #endregion

    private float slowdownFactor;

    public bool IsScaled()
    {
        return isScaled;
    }

    void Awake()
    {
        CheckScale();
    }

    public void RunFor(float delay = 1f, float factor = 0.05f, Action onComplete = null)
    {
        CheckScale();
        slowdownFactor = factor;

        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * oriFixedDeltaTime;

        StartCoroutine(DelayFunc(delay, () =>
        {
            ForceStop();
            onComplete?.Invoke();
        }));
    }

    public void Stop()
    {
        if (isScaled)
            ForceStop();
    }

    private void ForceStop()
    {
        Time.timeScale = oriTimeScale;
        Time.fixedDeltaTime = oriFixedDeltaTime;
        isScaled = false;
    }

    private IEnumerator DelayFunc(float delay, Action func)
    {
        yield return new WaitForSecondsRealtime(delay);
        func?.Invoke();
    }

    private void CheckScale()
    {
        if (!isScaled)
        {
            oriTimeScale = Time.timeScale;
            oriFixedDeltaTime = Time.fixedDeltaTime;
            isScaled = true;
        }
    }
}
