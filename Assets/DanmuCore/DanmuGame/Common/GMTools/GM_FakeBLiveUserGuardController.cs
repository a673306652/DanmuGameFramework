using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Daan;
using DG.Tweening;
using OpenBLive.Runtime.Data;

namespace DanMuGame
{
    public class GM_FakeBLiveUserGuardController : MonoBehaviour
    {
        [HideInInspector]
        public BilibiliDanMuClient client;
        public long uid;
        public string nickname;
        public long level;
        public bool send;

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
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                this.send = true;
            }
            
            if (this.send)
            {
                //this.client.BLiveGuardBuyMessageHandlerAsync(new Guard()
                //{
                //    userInfo = new UserInfo()
                //    { 
                //        uid = this.uid,
                //        userName = this.nickname,
                //    },
                //    guardLevel = this.level,
                //});
                this.send = false;
            }
        }
    }
}