using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using Daan;
using DouyuDanmu;
using DanMuGame;
using SuperSocket.ClientEngine;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
public struct DANMU_PACKAGE
{
    /// <summary>
    /// 长度
    /// </summary>
    public int dwLen;
    /// <summary>
    /// 长度
    /// </summary>
    public int dwLen2;
    /// <summary>
    /// 发送方向
    /// </summary>
    public Int16 bType;
    /// <summary>
    /// 加密字段（保留）
    /// </summary>
    public byte encrypt;
    /// <summary>
    /// 备注字段（保留）
    /// </summary>
    public byte reserved;
}

// <summary>
/// 套接字数据
/// </summary>
public class SOCKET_PACKAGE
{
    /// <summary>
    /// Socket套接字主对象
    /// </summary>
    public Socket Socket = null;
    /// <summary>
    /// 缓冲区大小
    /// </summary>
    public const int BufferSize = 4;    // 说明一下，这里由于有的包并不够1024缓冲区，经过大量测试，缓冲区设置为4最合适了
    /// <summary>
    /// 套接字缓冲区
    /// </summary>
    public byte[] SocketBuffer = new byte[BufferSize];
    /// <summary>
    /// 套接字流缓存
    /// </summary>
    public NetworkStream Stream = null;
}

public static class SocketHelper
{
    /// <summary>
    /// 发送斗鱼报文
    /// </summary>
    /// <param name="message"></param>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static void LiveMessagePush(string message, NetworkStream ms)
    {
        DANMU_PACKAGE package = new DANMU_PACKAGE();
        package.bType = 689;
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        package.dwLen = buffer.Length + 8;
        package.dwLen2 = package.dwLen;
        package.encrypt = 0x00;
        package.reserved = 0x00;

        byte[] block = new byte[buffer.Length + 12];
        Array.Copy(StreamSerializationHelper.StructureToBytes(package), 0, block, 0, 12);
        Array.Copy(buffer, 0, block, 12, buffer.Length);
        ms.Write(block, 0, block.Length);
        ms.Flush();
    }
}

/// <summary>
/// 本基类提供和二进制结构体数据处理的相关函数，这里包含的所有方法都是与标准语言二进制结构体操作
/// 相关函数
/// </summary>
/// <remarks>
/// 本基类提供和二进制结构体数据处理的相关函数。这里采用静态方法的形式提供出各种数据对象进行互转
/// 的方法
/// <list type="bullet">
/// <item>二进制文件到结构体的转换</item>
/// <item>结构体文件转换为二进制数据</item>
/// </list>
/// </remarks>
public static class StreamSerializationHelper
{
    /// <summary>
    /// 将托管格式结构体转换为byte数组格式
    /// </summary>
    /// <param name="graph">源数据</param>
    /// <returns></returns>
    public static byte[] StructureToBytes(object graph)
    {
        // 获取数据结构体大小（非托管）
        int dwStructureSize = Marshal.SizeOf(graph);
        // 从进程的非托管内存中分配内存
        IntPtr iter = Marshal.AllocHGlobal(dwStructureSize);
        // 将数据从托管对象封装送往非托管内存块
        Marshal.StructureToPtr(graph, iter, true);
        // 分配指定大小数组块
        byte[] mBytes = new byte[dwStructureSize];
        // 将数据从非托管内存复制到托管数组中
        Marshal.Copy(iter, mBytes, 0, dwStructureSize);
        Marshal.FreeHGlobal(iter);
        return mBytes;
    }
    /// <summary>
    /// 将非托管数组转换至托管结构体
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="graph">非托管数组</param>
    /// <returns></returns>
    public static T BytesToStructure<T>(byte[] graph)
    {
        // 获取数据结构体大小（托管）
        int dwStructureSize = Marshal.SizeOf(typeof(T));
        // 从进程的非托管内存中分配内存
        IntPtr iter = Marshal.AllocHGlobal(dwStructureSize);
        // 将数据从托管内存数组复制到非托管内存指针
        Marshal.Copy(graph, 0, iter, dwStructureSize);
        // 将数据从非托管内存块送到新分配并指定类型的托管对象并返回
        T obj = (T)Marshal.PtrToStructure(iter, typeof(T));

        Marshal.FreeHGlobal(iter);
        return obj;
    }

    /// <summary>
    /// 通过序列化复制对象
    /// </summary>
    /// <param name="graph"></param>
    /// <returns></returns>
    public static object CloneObject(object graph)
    {
        //ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

        using (MemoryStream memoryStream = new MemoryStream(1024))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(memoryStream, graph);

            memoryStream.Position = 0;

            return formatter.Deserialize(memoryStream);
        }
    }
}

public class DouyuClient
{
    private EasyClient client;
    private Timer ticker;

    public string Host = "danmuproxy.douyu.com";//"openbarrage.douyutv.com";
    public Int32 Port = 8601;
    public int roomId;

    public DouyuClient(int roomId, Action<Dictionary<string, object>> messageHandler)
    {
        this.roomId = roomId;
        client = new EasyClient();
        client.Initialize(new DouyuPackageFilter(), (package) =>
        {
            messageHandler(DouyuUtility.Deserialize(package.Data));
        });
    }

    public async Task Connect()
    {
        IPAddress[] ips = Dns.GetHostAddresses(Host);
        var connected = await client.ConnectAsync(new IPEndPoint(ips.First(), Port));

        if (connected)
        {
            Login();
            JoinGroup();
            ticker = Timer.Register(30, () => { Tick(); }, null, true);
        }
    }

    public void Send(Dictionary<string, object> args)
    {
        DouyuPackageInfo package = new DouyuPackageInfo();
        string argsStr = DouyuUtility.Serialize(args);
        byte[] data = package.Encode(argsStr);

        client.Send(new ArraySegment<byte>(data));
    }

    public void Login()
    {
        Send(
            new Dictionary<string, object>
            {
                    { "type", "loginreq" },
                    { "roomid", roomId }
            }
        );
    }

    public void JoinGroup()
    {
        Send(
            new Dictionary<string, object>
            {
                    { "type", "joingroup" },
                    { "rid", roomId },
                    { "gid", -9999 }
            }
        );
    }

    public void Tick()
    {
        Send(
            new Dictionary<string, object>
            {
                    { "type", "keeplive" },
                    { "tick", DouyuUtility.UnixTimestamp().ToString() }
            }
        );
    }

    public void Logout()
    {
        Send(
            new Dictionary<string, object>
            {
                    { "type", "logout" }
            }
        );
    }

    public void Close()
    {
        if (ticker != null)
        {
            ticker.Cancel();
        }

        if (client != null && client.IsConnected)
        {
            Logout();
            client.Close();
        }
    }
}
