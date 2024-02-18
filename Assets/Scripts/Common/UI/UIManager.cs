using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : HisaoMono
{

    public static UIManager Instance;
    public Camera BaseOverlayCam;
    public Camera OverlayCam;
    public Camera OverlayUpCam;

    private void Awake()
    {
        Instance = this;
    }
}
