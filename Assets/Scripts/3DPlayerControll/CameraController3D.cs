using System;
using System.Collections;
using System.Collections.Generic;
using Hisao;
using UnityEngine;

public class CameraController3D : MonoBehaviour
{
    public GameObject cameraTarget;
    public float offset = 10f;
    public Vector3 v3off;
    private Vector3 tempV3off;
    
    public static bool LockState;

    private float tempX;
    private float tempY;
    private Vector3 tempDir;

    public float Xspeed = 1;
    public float Yspeed =1;

    public static CameraController3D Instance;

    private void Awake()
    {
        Instance = this;
        tempX = 60;
        tempDir = new Vector3(60, 0, 0);
        Cursor.lockState = CursorLockMode.Locked;
        var centerPos = cameraTarget.transform.position;
        transform.position = centerPos - tempDir*offset;
        transform.forward = tempDir;
    }

    private void Update()
    {
         //先定义一个一直跟随目标的中心点坐标
         var centerPos = cameraTarget.transform.position;
         tempX -= Input.GetAxis("Mouse Y") * Yspeed;
         tempY += Input.GetAxis("Mouse X") * Xspeed;
         tempX = Mathf.Clamp(tempX, -60, 60);
         var dir = new Vector3(tempX, tempY, 0).Euler2Dir();
         tempDir = Vector3.Lerp(tempDir, dir, 0.2f);
         transform.position = centerPos - tempDir*offset + transform.forward* v3off.z + transform.right* v3off.x + transform.up * v3off.y;
         transform.forward = tempDir;

    }
}
