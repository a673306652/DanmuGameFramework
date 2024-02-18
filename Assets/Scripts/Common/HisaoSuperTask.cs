using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HisaoSuperTask : SingletonForHisaoMono<HisaoSuperTask>
{
    public List<SuperHisaoTask> tasks;

    private List<SuperHisaoTask> sleepingTasks;
    private List<SuperHisaoTask> tempTask;

    public void Init()
    {
        tasks = new List<SuperHisaoTask>();
        sleepingTasks = new List<SuperHisaoTask>();
        tempTask = new List<SuperHisaoTask>();
    }

    public void EnqueueRemoveTask(SuperHisaoTask task)
    {
        tempTask.Add(task);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="执行行为"></param>
    /// <param name="延迟启动的时间"></param>
    /// <param name="循环次数，-1表示无限循环"></param>
    /// <returns></returns>
    public SuperHisaoTask EnqueueTask(UnityAction<SuperHisaoTask> ac, float time, int loopCount = 0)
    {
        var t = GetTask();
        t.action = ac;
        t.targetTime = time;
        t.tick = 0;
        t.loopCount = loopCount;
        tasks.Add(t);
        return t;
    }

    public SuperHisaoTask DelayEnqueueTask(UnityAction<SuperHisaoTask> ac, float time, float startDelay,
        int loopCount = 0)
    {
        var t = GetTask();
        t.action = ac;
        t.targetTime = time;
        t.tick = 0;
        t.loopCount = loopCount;
        Exec((a) => { tasks.Add(t); }, startDelay);
        return t;
    }

    public void UpdateTask(float delta)
    {
        if (tempTask.Count > 0)
        {
            for (var i = 0; i < tempTask.Count; i++)
            {
                tasks.Remove(tempTask[i]);
                sleepingTasks.Add(tempTask[i]);
                tempTask[i].Clear();
            }

            tempTask.Clear();
        }

        var t = DateTime.UtcNow.Ticks;

        for (var i = 0; i < tasks.Count; i++)
        {
            tasks[i].tick += delta;
            if (tasks[i].tick >= tasks[i].targetTime)
            {
                try
                {
                    tasks[i].action.Invoke(tasks[i]);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }

                if (tasks[i].loopCount == -1)
                {
                    //一开始就是-1就说明是无限循环类型
                    tasks[i].tick = 0;
                }
                else
                {
                    tasks[i].loopCount -= 1;
                    if (tasks[i].loopCount < 0)
                    {
                        tempTask.Add(tasks[i]);
                    }
                    else
                    {
                        tasks[i].tick = 0;
                    }
                }
            }
        }

        if (tempTask.Count > 0)
        {
            for (var i = 0; i < tempTask.Count; i++)
            {
                tasks.Remove(tempTask[i]);
                sleepingTasks.Add(tempTask[i]);
                tempTask[i].Clear();
            }

            tempTask.Clear();
        }
    }

    public SuperHisaoTask GetTask()
    {
        if (sleepingTasks.Count > 0)
        {
            var m = sleepingTasks[0];
            sleepingTasks.RemoveAt(0);
            return m;
        }
        else
        {
            return new SuperHisaoTask();
        }
    }
}

public class SuperHisaoTask
{
    public UnityAction<SuperHisaoTask> action;
    public float targetTime;
    public float tick;
    public int loopCount;
    public HisaoMono owner;

    public void Clear()
    {
        action = null;
        targetTime = 0;
        tick = 0;
        loopCount = 0;
        if (null != owner)
        {
            owner.OnTaskInvoked(this);
        }

        owner = null;
    }

    public void Stop()
    {
        HisaoSuperTask.Instance.EnqueueRemoveTask(this);
    }
}