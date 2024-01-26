using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HisaoMono : MonoBehaviour
{
    public static float GLOBAL_TIME = 1f;

    protected float LOCAL_TIME = 1f;
    
    public HisaoTask Exec(UnityAction oc, float delay)
    {
        return new HisaoTask(StartCoroutine(RunTask(oc, delay)), this);
    }

    private IEnumerator RunTask(UnityAction oc, float delay)
    {
        float ticker = 0f;

        while (ticker<delay)
        {
            ticker += Time.fixedDeltaTime * LOCAL_TIME * GLOBAL_TIME;
            yield return new WaitForFixedUpdate();
        }
        oc.Invoke();
    }
    
}

public class HisaoTask
{
    private Coroutine task;
    private HisaoMono runningObj;

    public HisaoTask(Coroutine t,HisaoMono obj)
    {
        task = t;
        obj = runningObj;
    }
    
    public void Stop()
    {
        if (null!=task)
        {
            runningObj.StopCoroutine(task);
            task = null;
        }
    }
}