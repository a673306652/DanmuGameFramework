using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToCamera : MonoBehaviour
{
    public bool awalys = false;

    private void Awake()
    {
        this.transform.forward = -Camera.main.transform.forward;
    }

    private void Update()
    {
        if (this.awalys)
        {
            this.transform.forward = -Camera.main.transform.forward;
        }
    }
}
