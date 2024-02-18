using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AlwaysUp : MonoBehaviour
{

    void Update()
    {
        this.transform.up = Vector3.up;
    }
}

