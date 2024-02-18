using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
using DanMuGame;
using System.Threading.Tasks;
using OpenBLive.Runtime.Data;

public class BilibiliGetFakeUserClient : BilibiliDanMuClient
{
    private GameObject parent;

    public List<string> nameList = new List<string>();
    public List<string> faceList = new List<string>();

    void Start()
    {
        this.parent = new GameObject("FakeUserList");
    }

    public override void DanmuMessageHandlerAsync(string uid, string nickname, string content, string faceUrl)
    {
        if (!this.nameList.Contains(nickname))
        {
            this.nameList.Add(nickname);
            this.faceList.Add(faceUrl);
        }
    }
    [ContextMenu("获取假用户")]
    public void GetFakeUserList()
    {
        this.parent.transform.DestroyAllChildren();
        for (int i = 0; i < this.nameList.Count; i++)
        {
            var name = this.nameList[i];
            var face = this.faceList[i];

            var obj = new GameObject();
            obj.transform.SetParent(this.parent.transform);
            obj.name = $"{name}|{face}";
        }
    }
}
