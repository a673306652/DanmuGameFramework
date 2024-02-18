using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Patterns;
using UnityEngine;
using UnityEngine.Events;

public class HisaoMono : MonoBehaviour, PoolableObject
{
    public static float GLOBAL_TIME = 1f;

    protected float LOCAL_TIME = 1f;

    public List<SuperHisaoTask> runningTask = new List<SuperHisaoTask>();

    public void OnTaskInvoked(SuperHisaoTask task)
    {
        try
        {
            runningTask.Remove(task);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void StopAllTask()
    {
        for (var i = 0; i < runningTask.Count; i++)
        {
            runningTask[i].Clear();
        }
    }
    
    public HisaoTask Exec(UnityAction<SuperHisaoTask> oc, float delay)
    {
        if (delay<0)
        {
            delay = 0;
        }

        var xm = new HisaoTask(HisaoSuperTask.Instance.EnqueueTask(oc, delay), this);
        runningTask.Add(xm.task);
        return xm;
    }
    
    public HisaoTask ExecFTime(UnityAction<SuperHisaoTask> oc, float delay)
    {
        if (delay<0)
        {
            delay = 0;
        }
        var xm = new HisaoTask(HisaoSuperTask.Instance.EnqueueTask(oc, delay), this);
        runningTask.Add(xm.task);
        return xm;
    }
    public HisaoTask ExecG(UnityAction<SuperHisaoTask> oc, float delay)
    {
        if (delay<0)
        {
            delay = 0;
        }var xm = new HisaoTask(HisaoSuperTask.Instance.EnqueueTask(oc, delay), this);
        runningTask.Add(xm.task);
        return xm;
    }
    
    public HisaoTask ExecM(UnityAction<SuperHisaoTask> oc,int loopCount, float startDelay,float delay)
    {
        if (delay<0)
        {
            delay = 0;
        }
        if (startDelay<0)
        {
            startDelay = 0;
        }

        if (loopCount<0)
        {
            loopCount = 0;
        }
        
        var b = new HisaoTask();
        b.task =  HisaoSuperTask.Instance.DelayEnqueueTask(oc, delay,startDelay,loopCount);
        b.runningObj = this;
        runningTask.Add(b.task);
        return b;
    }

    private IEnumerator RunMediator(int loop, UnityAction<HisaoTask> oc, float startDelay, float loopPerTime,HisaoTask task)
    {
        yield return new WaitForSeconds(startDelay);
        int cLoop = 0;
        while (cLoop<loop)
        {
            cLoop++;
            float ticker = 0f;

            while (ticker<loopPerTime)
            {
                ticker += Time.fixedDeltaTime * LOCAL_TIME * GLOBAL_TIME;
                yield return new WaitForFixedUpdate();
            }
            oc.Invoke(task);
        }
    }
    
    private IEnumerator RunTaskG(UnityAction oc, float delay)
    {
        float ticker = 0f;

        while (ticker<delay)
        {
            ticker += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        oc.Invoke();
    }

    private IEnumerator RunTaskFTime(UnityAction oc, float delay)
    {
        float ticker = 0f;

        while (ticker<delay)
        {
            ticker += Time.fixedDeltaTime * LOCAL_TIME * GLOBAL_TIME;
            yield return new WaitForUpdate();
        }
        oc.Invoke();
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

    public virtual void OnObjectReset()
    {
         
    }

    public virtual void OnObjectRecycle()
    {
       
    }
}

public class HisaoTask
{
    public SuperHisaoTask task;
    public HisaoMono runningObj;

    public HisaoTask(SuperHisaoTask t,HisaoMono obj)
    {
        task = t;
        obj = runningObj;
    }
    public HisaoTask()
    {
        
    }
    public void Stop()
    {
        if (null!=task)
        {
            task.Stop();
            task = null;
        }
    }
}