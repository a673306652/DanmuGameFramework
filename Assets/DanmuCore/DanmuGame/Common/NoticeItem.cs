using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NoticeItem : MonoBehaviour
{
    private UnityAction OnStart;
    private UnityAction OnComplete;
    private int _Priority;
    private float _Duration;
    private string[] _Keys;
    public int TaskID;
    public int Count;
    public string Id;

    private Timer currentTimer;
    public int Priority
    {
        get { return _Priority; }
    }

    public string[] Keys
    {
        get
        {
            return _Keys;
        }
    }

    public void SetExComplete(UnityAction complete)
    {
        UnityAction task = () =>
        {
            complete.Invoke();
            OnComplete.Invoke();
        };
        OnComplete = task;
    }

    public void StopLifeTimeTaskAndRestart(UnityAction run, float duration)
    {
        run.Invoke();
        if (null != currentTimer)
        {
            currentTimer.Cancel();
            currentTimer = null;
        }
        currentTimer = Timer.Register(duration, () =>
        {
            OnComplete.Invoke();
        });

    }

    public NoticeItem SetNoticeTask(string[] keys, UnityAction Start, UnityAction Complete, int priority, int taskID, float duration = -1)
    {
        OnStart = Start;
        OnComplete = Complete;
        _Priority = priority;
        _Duration = duration;
        _Keys = keys;
        TaskID = taskID;

        return this;
    }

    public void Run()
    {

        OnStart.Invoke();
        if (_Duration > 0)
        {
            currentTimer = Timer.Register(_Duration, () =>
             {
                 OnComplete.Invoke();
             });
        }

    }

    public void Complete()
    {
        OnComplete.Invoke();
    }

    public override bool Equals(object other)
    {
        if (null == other) return false;
        if ((other as NoticeItem) == null) return false;

        return (other as NoticeItem).TaskID == TaskID;
    }

    public override int GetHashCode()
    {
        return TaskID;
    }
}