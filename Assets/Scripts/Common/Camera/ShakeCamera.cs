using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShakeCamera : HisaoMono

{
    public static ShakeCamera Instance;
    private Vector3 origin;
    public float intensity;
    public float value;

    private void Awake()
    {
        Instance = this;
        origin = transform.localPosition;
    }

    [Button()]
    public void Test()
    {
        Shake(1f, 0.3f);
    }

    public void Shake(float time, float intensity)
    {
        this.value = time;
        this.intensity = intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (value > 0)
        {
            value -= Time.deltaTime;
            var dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            transform.localPosition = origin + dir * intensity * value;
        }
        else
        {
            transform.localPosition = origin;
        }
    }
}