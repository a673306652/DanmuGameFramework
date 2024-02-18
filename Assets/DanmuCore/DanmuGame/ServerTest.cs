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
        Debug.Log($"[{DateTime.Now}] [ServerInfo]: ��������LoliSocketServer������...");
        Server = new LoliSocketServer("127.0.0.1", 41414);
        Server.SocketAccept += Server_SocketAccept; //�յ��ͻ������� �¼�
        Server.SocketReceive += Server_SocketReceive;   //�յ��ͻ��˷��͹����˵���Ϣ �¼�
        var isStart = Server.Start();
        if (isStart)
        {
            Debug.Log($"[{DateTime.Now}] [ServerInfo]: LoliSocketServer�����������ɹ�!");
        }
        else
        {
            Debug.LogError($"[{DateTime.Now}] [ServerError]: LoliSocketServer����������ʧ��!");
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"[{DateTime.Now}] [ServerInfo]: ���ڹر�LoliSocketServer������...");
        if (Server.Close())
        {
            Debug.Log($"[{DateTime.Now}] [ServerInfo]: �ѹر�LoliSocketServer������!");
        }
        else
        {
            Debug.Log($"[{DateTime.Now}] [ServerError]:LoliSocketServer�������ر�ʧ��!");
        }
    }


    //����пͻ��˷�����Ϣ �ͻ�ִ�����
    private void Server_SocketReceive(object sender, ServerReceiveEventArgs e)
    {
        string Message = Encoding.UTF8.GetString(e.Data, 0, e.ActualLength);
        //��Ϣʾ��: �����û�UID ��Ļ��Ϣ ͷ��·��
        msg = Message.Split('\0');
        //PELog.ColorLog(LogColor.Yellow,msg);
        //Debug.Log($"�յ�����UID: {msg[0]} ����Ϣ: {msg[1]} Path: {msg[2]}");
        run = true;

    }
    //����пͻ������� �ͻ�ִ�����
    private void Server_SocketAccept(object sender, ServerAcceptEventArgs e)
    {
        //Debug.Log($"[{DateTime.Now}] [ServerInfo]: NEW Client Login: {e.ClientInfo.ID}");
    }

}
