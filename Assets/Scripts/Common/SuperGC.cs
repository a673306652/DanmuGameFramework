using System;
using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;

public  class SuperGC : SingletonForHisaoMono<SuperGC>
{
   
    private List<ShitGC> activeShit = new List<ShitGC>();
    private float currentTime;
    private class ShitGC
    {
        public string PoolName;
        public GameObject Body;
        public float RecordTime;
        public float Duration;
    }

    private void Awake()
    {
 
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        for (var i = 0; i < activeShit.Count; i++)
        {
            if (currentTime- activeShit[i].RecordTime > activeShit[i].Duration)
            {
                var pool = HisaoResourcesPool.Instance ;
               
                pool.ReturnOne(activeShit[i].PoolName,activeShit[i].Body);
                activeShit.RemoveAt(i);
            }
        }
    }

    public void EnqueueShitGC(string poolName,GameObject Body,float Duration)
    {
        var shitGc = new ShitGC();
        shitGc.Body = Body;
        shitGc.PoolName = poolName;
        shitGc.Duration = Duration;
        shitGc.RecordTime = currentTime;
        activeShit.Add(shitGc);
    }
}


