using System;
using System.Collections;
using System.Collections.Generic;
using Hisao;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(KeyboradInput))]
public class PlayerControll3D : HisaoMono
{
    public static PlayerControll3D Instance;

    private Rigidbody rig;
    private bool onMove;
    private bool onGround;

    [SerializeField] private KeyboradInput ki;

    [SerializeField] private float _Speed = 10;
    [SerializeField] private float JumpVelocity = 400;
    [SerializeField] private DirMode MoveMode;
    [SerializeField] private float G = 0.5f;
    
    private float tempSpeed;
   

    private void Awake()
    {
        Instance = this;
        rig = GetComponent<Rigidbody>();
        ki = GetComponent<KeyboradInput>();
        tempSpeed = _Speed;
        onGround = true;
    }

    private void FixedUpdate()
    {
        ki.TickKey();
        ki.SetDup();

        if (Mathf.Abs(ki.GetDup().x) > 0.01f || Mathf.Abs(ki.GetDup().y) > 0.01f)
        {
            onMove = true;
        }

        if (Mathf.Abs(ki.GetDup().x) < 0.01f && Mathf.Abs(ki.GetDup().y) < 0.01f)
        {
            onMove = false;
        }

        var t = new Vector3(ki.GetDup().y, 0, ki.GetDup().x);
        var goPower = Mathf.Max(Mathf.Abs(ki.GetDup().x), Mathf.Abs(ki.GetDup().y));
        var dir = t.GlobalDir2LocalDir2D(Camera.main.transform.forward).normalized;

        if (MoveMode != DirMode.WithCameraForward)
        {
            dir = Camera.main.transform.forward.xz();
            dir.x *= t.x;
            dir.z *= t.y;
        }

        rig.AddForce(dir * (ki.shift.IsPressing ? 0.7f : 0.5f) * _Speed, ForceMode.VelocityChange);
        rig.velocity =
            new Vector3(Mathf.Clamp(rig.velocity.x, -_Speed * 2, _Speed * 2) * (ki.shift.IsPressing ? 0.7f : 0.5f),
                rig.velocity.y,
                Mathf.Clamp(rig.velocity.z, -_Speed * 2, _Speed * 2) * (ki.shift.IsPressing ? 0.7f : 0.5f));
        rig.velocity -= Vector3.up * G;

        if (onMove)
        {
            var tQ = dir.Dir2Quaternion();
            transform.rotation = Quaternion.Lerp(transform.rotation, tQ, 0.2f);
        }

        if (ki.space.OnPressed && onGround)
        {
            jump();
        } 
        
        if (onGround)
        {
            _Speed = tempSpeed;
         
        }
        else
        {
            _Speed = tempSpeed * 0.6f;
        }

    }

    private HisaoTask jumpTask;

    public void jump()
    {
        if (jumpTask != null)
        {
            jumpTask.Stop();
            jumpTask = null;
        }
        jumpTask = this.Exec((a) =>
        {
            onGround = true;
            jumpTask = null;
        }, 0.1f);
        onGround = false;
        rig.AddForce(Vector3.up * JumpVelocity);
        rig.velocity -= new Vector3(rig.velocity.x * 0.2f, 0, rig.velocity.z * 0.2f);
    }

    void Update()
    {
    }


    private enum DirMode
    {
        WithCameraForward,
        ScreenSpaceDir
    }
}