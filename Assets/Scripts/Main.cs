using System;
using System.Collections;
using System.Collections.Generic;
using Daan;
using DanMuGame;
using NaughtyAttributes;
using UnityEngine;

public class Main : HisaoMono
{
    public static Main Instance;
    
 

    private void Awake()
    {
        Instance = this;
        FakeUserManager.Instance.Initialize();
        AddCMD();
        //订阅广播事件
        Message.Main.AddListener(Message.SystemHints.ConnectionSuccess, OnConnectionSucced);
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

    private void AddCMD()
    {
        DanmuCommandManager.Instance.talkCommand = (id, content, count) =>
        {
            
                TTTestTool.Instance.FakeTalk(id,content);
           
            return false;
        }; 
        DanmuCommandManager.Instance.AddCommand("加入", (id, content, count) =>
            {
                //这里会提供给你发送这个消息的玩家的id 
                Debug.LogError(id+"加入了游戏");
              
                return false;
            }
        );
        DanmuCommandManager.Instance.AddLikeCommand((id,content,count) =>
        {
            
            return false;
        });
        
        DanmuCommandManager.Instance.AddGiftNotice("棒棒糖",
            (id, content, count, origin) =>
            {
                
                Debug.LogError(id+"送出棒棒糖");
                //这里会提供 赠送该礼物的用户，组刷的数量
                //需要注意，这里不止需要在这里添加该礼物指令的实际作用，同时需要在Client类中的Gift Config类配置礼物名称，否则不会执行这个指令，而是执行未知礼物指令
            });
 
    }
}
