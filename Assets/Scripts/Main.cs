using System;
using System.Collections;
using System.Collections.Generic;
using Daan;
using DanMuGame;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class Main : HisaoMono
{
    public static Main Instance;
    //这里的Main是演示

    private void Awake()
    {
        Instance = this;
        GlobalSystemController.Init();
        //所有其他模块都在这里初始化
        AddCMD();
        //订阅广播事件
        Message.Main.AddListener(Message.SystemHints.ConnectionSuccess, OnConnectionSucced);
    }

    private void Start()
    {
  
        TTTestTool.Instance.FakeTalk(FakeUserManager.Instance.GetFakeUser().uid, "开始无限循环的定时协程，在10秒后结束");
        var x = HisaoSuperTask.Instance.EnqueueTask(
            (a) => { TTTestTool.Instance.FakeTalk(FakeUserManager.Instance.GetFakeUser().uid, "一条来自协程的消息"); }, 1f, -1);
        HisaoSuperTask.Instance.EnqueueTask((a) =>
        {
            x.Stop();
            TTTestTool.Instance.FakeTalk(FakeUserManager.Instance.GetFakeUser().uid, "定时协程，已结束循环任务");
        }, 10);
    }

    [Button()]
    private void StartGame()
    {
        Message.Main.Broadcast(Message.SystemHints.Connecting);
    }

    private void OnConnectionSucced(object obj)
    {
        //连接成功后该做的事
        Debug.Log("Connet succeed");
    }

    private void Update()
    {
        var delta = Time.deltaTime * GLOBAL_TIME;
        GlobalSystemController.Update(delta);
    }

    private void AddCMD()
    {
        DanmuCommandManager.Instance.talkCommand = (id, content, count) =>
        {
            TTTestTool.Instance.FakeTalk(id, content);

            return false;
        };
        DanmuCommandManager.Instance.AddCommand("加入", (id, content, count) =>
            {
                //这里会提供给你发送这个消息的玩家的id 
                Debug.LogError(id + "加入了游戏");

                return false;
            }
        );
        DanmuCommandManager.Instance.AddLikeCommand((id, content, count) => { return false; });

        DanmuCommandManager.Instance.AddGiftNotice("棒棒糖",
            (id, content, count, origin) =>
            {
                Debug.LogError(id + "送出棒棒糖");
                //这里会提供 赠送该礼物的用户，组刷的数量
                //需要注意，这里不止需要在这里添加该礼物指令的实际作用，同时需要在Client类中的Gift Config类配置礼物名称，否则不会执行这个指令，而是执行未知礼物指令
            });
    }
}