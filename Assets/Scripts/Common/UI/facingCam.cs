using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class facingCam : MonoBehaviour
{
    [Header("[null will reference main camera.]")]
    [SerializeField]private Camera mainCam;
    public bool isInvert;
    public bool isOrth;
    // Start is called before the first frame update
    void Awake()
    {
        if(mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(mainCam == null)
        {
            mainCam = Camera.main;
        }
        var camPos = mainCam.transform.position;

        var t2c = (camPos - transform.position).normalized;
        if (isOrth)
        {
            t2c = -mainCam.transform.forward;
        }

        transform.forward = isInvert?-t2c:t2c;

    }
}
