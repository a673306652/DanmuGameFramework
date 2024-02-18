namespace DanMuGame
{
    using System.Collections;
    using System.Collections.Generic;
    using Modules.Patterns;
    using Modules.Utilities;
    using UnityEngine;

    public class FakeUserManager : MonoSingleton<FakeUserManager>
    {
        public class Data
        {
            public string uid;
            public string nickname;
            public string faceUrl;
        }

        private static int FakeID;

        private List<Data> list = new List<Data>();
        private Dictionary<string, Data> dict = new Dictionary<string, Data>();
        public Dictionary<string, Data> existFakeUserDict = new Dictionary<string, Data>();

        public void Initialize()
        {
            // var fakeUsers = GetComponentsInChildren<GM_FakeCommandBase>(true);
            // foreach (var u in fakeUsers)
            // {
            //     this.AddUser(u.uid, u.nickname, u.faceUrl);
            // }
            FakeID = 0;
            var list = Resources.LoadAll<Transform>("FakeUser");
            foreach (var item in list)
            {
                foreach (Transform child in item)
                {
                    var str = child.name.Split('|');
                    this.AddUser("Fake"+FakeID,str[0], str[1]);
                    FakeID++;
                }
            }
            Debug.Log( this.existFakeUserDict.Count);
        }

        public void AddUser(string uid, string nickname, string faceUrl)
        {
            if (this.dict.ContainsKey(uid)) return;
            if (!string.IsNullOrEmpty(faceUrl))
            {
                faceUrl = faceUrl.Replace("\r", "");
            }
            var data = new Data()
            {
                uid = uid,
                nickname = nickname,
                faceUrl = faceUrl,
            };
            this.list.Add(data);
            this.dict.Add(uid, data);
            this.existFakeUserDict.Add(uid,data);
        }

        public Data GetFakeUser()
        {
            Data data = null;
            if (this.list.Count > 0)
            {
                data = this.list[RandomUtils.Range(0, this.list.Count)];
            }
            if (data == null)
            {
                Debug.LogWarning("没有用户数据");
            }
            return new Data() { nickname = data.nickname, faceUrl = data.faceUrl,uid = data.uid};
        }

        public void Clear()
        {
            this.list.Clear();
        }
    }

}