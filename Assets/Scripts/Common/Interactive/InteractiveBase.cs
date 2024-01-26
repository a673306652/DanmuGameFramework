using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class InteractiveBase : MonoBehaviour
{
    public Coroutine myTick;
    public UnityAction OnInteractive;
    public abstract void OnStart();
    public abstract void OnUpdate();

    public void EndInteractive()
    {
       
    }
    
    
}
