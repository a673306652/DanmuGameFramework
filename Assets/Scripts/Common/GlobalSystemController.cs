using System.Collections;
using System.Collections.Generic;
using DanMuGame;
using UnityEngine;

public static class GlobalSystemController 
{
    public static void Init()
    {
        FakeUserManager.Instance.Initialize();
        HisaoSuperTask.Instance.Init();
        RollTool.Init();
    }

    public static void Update(float delta)
    {
        HisaoSuperTask.Instance.UpdateTask(delta);
    }
}
