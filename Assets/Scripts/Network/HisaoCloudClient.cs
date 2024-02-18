using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine;

public class HisaoCloudClient : HisaoMono
{
    private Socket socket;

    public static HisaoCloudClient Instance;
    public int CLIENT_ID;

    public byte[] buffer = new byte[10240];
    // Start is called before the first frame update

    public bool 本地服务器;
    private string currentIP;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Connect();
    }

    void Connect()
    {
         
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        if (本地服务器)
        {
            socket.Connect("127.0.0.1", 25565);
            currentIP = "127.0.0.1";
        }
        else
        {
            socket.Connect("47.98.189.96", 25565);
            currentIP = "47.98.189.96";
        }


        StartReceive();
        Debug.Log("开始连接服务器" + currentIP);
        Send("link_client", "Link!");
    }

    void Close()
    {
        if (null != HeartBeatTask)
        {
            HeartBeatTask.Stop();
        }

        if (null != HeartBeatFailedTask)
        {
            HeartBeatFailedTask.Stop();
        }

        try
        {
            socket.Disconnect(false);
        }
        catch (Exception e)
        {
           Debug.LogError(e);
       
        }
       
    }

    public bool isLinked;


    private void OnApplicationQuit()
    {
        var op = new HisaoMessage();
        op.Content = string.Empty;
        op.ClinetID = CLIENT_ID.ToString();
        op.MessageType = "quit";
        var xp = JsonConvert.SerializeObject(op);
        socket.Send(Encoding.UTF8.GetBytes(xp));
        socket.Disconnect(false);
    }

    //尝试一个心跳包，客户端每10秒发送一次心跳包给服务器，如果发送后30秒内未收到回复，则判断与服务器失联，不断尝试重连

    private void OnLinkSucceed()
    {
        isLinked = true;

        HeartBeatTask = HisaoSuperTask.Instance.EnqueueTask((a) => { Send("heart_beat", "Link!"); }, 10);
        Debug.Log("发送心跳包");
        HeartBeatFailedTask = HisaoSuperTask.Instance.EnqueueTask((a) =>
        {
            isLinked = false;
            Debug.Log("心跳失败，开始重连");
            RelinkTask = HisaoSuperTask.Instance.EnqueueTask((a) =>
            {
                if (isLinked)
                {
                    Debug.Log("重连成功");
                    a.Stop();
                    RelinkTask = null;
                    return;
                }

                Close();
                Connect();
            }, 5, -1);
            //三十秒后未中断此事件，说明失联，尝试重新连接
        }, 30);
    }

    private void Update()
    {
    }

    private void OnHeartBeatSucceed()
    {
        isLinked = true;
        HeartBeatFailedTask.Stop();
        HeartBeatFailedTask = null;

        HeartBeatTask = HisaoSuperTask.Instance.EnqueueTask((a) => { Send("heart_beat", "Link!"); }, 10);
        Debug.Log("发送心跳包");
        HeartBeatFailedTask = HisaoSuperTask.Instance.EnqueueTask((a) =>
        {
            isLinked = false;
            Debug.Log("心跳失败，开始重连");
            RelinkTask = HisaoSuperTask.Instance.EnqueueTask((a) =>
            {
                if (isLinked)
                {
                    Debug.Log("重连成功");
                    a.Stop();
                    RelinkTask = null;
                    return;
                }
                Close();
                Connect();
            }, 5, -1);
            //三十秒后未中断此事件，说明失联，尝试重新连接
        }, 30);
    }

    private SuperHisaoTask HeartBeatTask;
    private SuperHisaoTask HeartBeatFailedTask;
    private SuperHisaoTask RelinkTask;

    public void StartReceive()
    {
        socket.BeginReceive(buffer, 00, buffer.Length, SocketFlags.None, OnReceived, null);
    }

    public void OnReceived(IAsyncResult iar)
    {
        int len = socket.EndReceive(iar);
        if (len == 0)
        {
            return;
        }

        string str = Encoding.UTF8.GetString(buffer, 0, len);

        var msg = JsonConvert.DeserializeObject<HisaoMessage>(str);

        Debug.Log("receive msg=> client_id: " + msg.ClinetID + " msg_type:" + msg.MessageType + " content:" +
                          msg.Content);
        //这里解析回调
        if (msg.MessageType.Equals("link_succeed"))
        {
            CLIENT_ID = int.Parse(msg.ClinetID);
            Debug.Log("Link succeed,Welcome back.");
            OnLinkSucceed();
        }

        if (msg.MessageType.Equals("heart_beat_succeed"))
        {
            
            CLIENT_ID = int.Parse(msg.ClinetID);
            Debug.Log("接收到心跳");
            OnHeartBeatSucceed();
        }

        StartReceive();
    }

    public void Send(string type, string str)
    {
        var d = new HisaoMessage();
        d.ClinetID = CLIENT_ID.ToString();
        d.MessageType = type;
        d.Content = str;
        var n = JsonConvert.SerializeObject(d);
        socket.Send(Encoding.UTF8.GetBytes(n));
    }
}