using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ilyfairy.LoliSocket;
using System;
using System.Text;

public class ServerTest : MonoBehaviour
{
    public LoliSocketServer Server { get; set; }
    private string[] msg;
    private bool run = false;

    void Start()
    {
        Debug.Log($"[{DateTime.Now}] [ServerInfo]: 正在启动LoliSocketServer服务器...");
        Server = new LoliSocketServer("127.0.0.1", 41414);
        Server.SocketAccept += Server_SocketAccept; //收到客户端连接 事件
        Server.SocketReceive += Server_SocketReceive;   //收到客户端发送过来了的信息 事件
        var isStart = Server.Start();
        if (isStart)
        {
            Debug.Log($"[{DateTime.Now}] [ServerInfo]: LoliSocketServer服务器启动成功!");
        }
        else
        {
            Debug.LogError($"[{DateTime.Now}] [ServerError]: LoliSocketServer服务器启动失败!");
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"[{DateTime.Now}] [ServerInfo]: 正在关闭LoliSocketServer服务器...");
        if (Server.Close())
        {
            Debug.Log($"[{DateTime.Now}] [ServerInfo]: 已关闭LoliSocketServer服务器!");
        }
        else
        {
            Debug.Log($"[{DateTime.Now}] [ServerError]:LoliSocketServer服务器关闭失败!");
        }
    }


    //如果有客户端发送信息 就会执行这个
    private void Server_SocketReceive(object sender, ServerReceiveEventArgs e)
    {
        string Message = Encoding.UTF8.GetString(e.Data, 0, e.ActualLength);
        //消息示例: 抖音用户UID 弹幕消息 头像路径
        msg = Message.Split('\0');
        //PELog.ColorLog(LogColor.Yellow,msg);
        //Debug.Log($"收到来自UID: {msg[0]} 的消息: {msg[1]} Path: {msg[2]}");
        run = true;

    }
    //如果有客户端连接 就会执行这个
    private void Server_SocketAccept(object sender, ServerAcceptEventArgs e)
    {
        //Debug.Log($"[{DateTime.Now}] [ServerInfo]: NEW Client Login: {e.ClientInfo.ID}");
    }

}
