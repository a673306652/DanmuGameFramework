using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;
using Daan;

public class StreamingAssetsLoader : Manager<StreamingAssetsLoader>
{
    public IEnumerator LoadTextureByPath(string path, Action<List<Texture2D>> action)
    {
        string ur = string.IsNullOrEmpty(path) ? Application.streamingAssetsPath : Application.streamingAssetsPath + "/" + path;
        var list = new List<string>();
        if (Directory.Exists(ur))
        {
            DirectoryInfo direction = new DirectoryInfo(ur);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                else
                {
                    //Debug.Log(ur + "/" + files[i].Name);  //打印出来这个文件夹下的所有文件
                    list.Add(ur + "/" + files[i].Name);
                }
            }
            return this.LoadTexture2D(list, action);
        }
        else
        {
            action?.Invoke(null);
        }
        return null;
    }


    IEnumerator LoadTexture2D(List<string> paths, Action<List<Texture2D>> action)
    {
        List<Texture2D> list = new List<Texture2D>();
        foreach (var path in paths)
        {
            UnityWebRequest requrest = UnityWebRequestTexture.GetTexture(path);
            yield return requrest.SendWebRequest();
            if (requrest.result == UnityWebRequest.Result.ConnectionError || requrest.result == UnityWebRequest.Result.ProtocolError || requrest.result == UnityWebRequest.Result.DataProcessingError)
            {
                //Debug.Log(requrest.error);
            }
            else
            {
                var name = Path.GetFileNameWithoutExtension(path);
                var texture = DownloadHandlerTexture.GetContent(requrest);
                texture.name = name;
                list.Add(texture);
            }
        }
        action?.Invoke(list);
    }


}
