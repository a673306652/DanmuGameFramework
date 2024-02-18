using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Daan;
using DG.Tweening;
using OpenBLive.Runtime.Data;

namespace DanMuGame
{
    public class GM_FakeBLiveUserGiftController : MonoBehaviour
    {
        public string key;
        [HideInInspector]
        public BilibiliDanMuClient client;
        public string uid;
        public string nickname;
        public string giftName;
        public int giftNum;
        public int giftPrice;
        public bool paid;
        public bool send;
        public string faceUrl;

        private void Awake()
        {
            if (this.client == null)
            {
                this.client = GameObject.FindObjectOfType<BilibiliDanMuClient>();
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (!string.IsNullOrEmpty(this.key) && Input.GetKeyDown(this.key))
            {
                this.send = true;
            }

            if (this.send)
            {
                var data = FakeUserManager.Instance.existFakeUserDict.TryGet(this.uid);
                if (data == null)
                {
                    var fakeData = FakeUserManager.Instance.GetFakeUser();
                    data = new FakeUserManager.Data();
                    data.nickname = string.IsNullOrEmpty(this.nickname) ? fakeData.nickname : this.nickname;
                    data.faceUrl = string.IsNullOrEmpty(this.faceUrl) ? fakeData.faceUrl : this.faceUrl;
                    FakeUserManager.Instance.existFakeUserDict.Add(this.uid, data);
                }

                this.client.GiftMessageHandlerAsync(this.uid, data.nickname, data.faceUrl, this.giftName, "0", this.giftPrice, this.giftNum);
                this.send = false;
            }
        }
    }
}