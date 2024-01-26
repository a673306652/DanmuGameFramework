using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    private Vector3 _newGravity = Physics.gravity;
    private Vector3 _compensate = Vector3.zero;
    private Rigidbody _rig;
    public float G;
    private void Start()
    {
        _rig = gameObject.GetComponent<Rigidbody>();
        Set(new Vector3(0,G,0));
    }
 
    public void Set(Vector3 val)
    {
        if (_newGravity == val)
            return;
 
        _newGravity = val;
        _compensate = -(Physics.gravity - val);
    }
 
    public Vector3 Get() { return _newGravity; }
 
    private void FixedUpdate()
    {
        Set(new Vector3(0,G,0));
        if (!_rig.isKinematic && !_rig.IsSleeping() && _compensate!=Vector3.zero)
        {
            _rig.AddForce(_compensate, ForceMode.Acceleration);
        }
    }
}
