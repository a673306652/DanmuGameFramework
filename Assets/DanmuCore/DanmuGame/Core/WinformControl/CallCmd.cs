using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Daan;
public class CallCmd : MonoBehaviour
{
    public static void SendCommond(string cmd)
    {
        var processInfo = new ProcessStartInfo("cmd.exe", "/S /K " + cmd)
        {
            CreateNoWindow = true,
            UseShellExecute = true,
            WindowStyle = ProcessWindowStyle.Normal,
        };

        var process = new Process { StartInfo = processInfo };
        process.Start();
    }
}
