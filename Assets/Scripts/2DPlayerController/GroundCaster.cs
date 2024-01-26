using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCaster : MonoBehaviour
{
    [SerializeField]private bool onGround;
    private float timeTick;

    private void Update()
    {
        if (onGround)
        {
            timeTick = 0;
        }
        if (PlayerController2p5D.instance.rb.velocity.y<=0.01f)
        {
            timeTick += Time.deltaTime;
        }
    
        if (timeTick>0.2f)
        {
            OutGround();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    public bool GetOnGround()
    {
        return onGround;
    }

    public void OnGround()
    {
        onGround = true;
    }

    public void OutGround()
    {
        onGround = false;
    }
}
