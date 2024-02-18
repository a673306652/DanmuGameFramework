using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Patterns;
using NaughtyAttributes;
using UnityEngine;

namespace DanMuGame
{
    public abstract class BaseDanmuClient : MonoBehaviour
    {
        public string platform;
        public long roomId;
        public string userId; // 内测客户端用户Id
        public float reconnectTime;
        protected bool readyToConnect = false;


        [ReadOnly]
        public bool 连接成功 = false;


        [System.Serializable]
        public class GiftConfig
        {
            public string origin;
            public List<CastConfig> castConfigs = new List<CastConfig>();
        }

        [System.Serializable]
        public class CastConfig
        {
            public string cast;
            public int castCount;
        }

        public List<GiftConfig> giftConfigsList = new List<GiftConfig>();
        public Dictionary<string, GiftConfig> giftConfigsDict = new Dictionary<string, GiftConfig>();

        public bool 未知礼物转换为礼物点 = true;


        public Dictionary<string, Sprite> giftSpriteDict = new Dictionary<string, Sprite>();

        protected virtual void Awake()
        {
            foreach (var item in this.giftConfigsList)
            {
                this.giftConfigsDict.Add(item.origin, item);
            }
            var list = Resources.LoadAll<Sprite>("Common/Gift");
            foreach (var item in list)
            {
                this.giftSpriteDict.Add(item.name, item);
            }
        }

        public DanMuUser GetUser(string uid)
        {
            return UserManager.Instance.GetUser(UserManager.GetUserKey(this.platform, this.roomId, uid));
        }
        public DanMuUser GetOnlineUser(string uid)
        {
            return UserManager.Instance.GetUser(UserManager.GetUserKey(this.platform, this.roomId, uid));
        }
        public string GetUserKey(string uid)
        {
            return UserManager.GetUserKey(this.platform, this.roomId, uid);
        }

        public string GetUserId(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }
            var splits = key.Split('/');
            if (splits.Length < 2) return key;
            return splits[2];
        }

        public string GetCleanUserId(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return raw;
            }
            var splits = raw.Split('/');
            if (splits.Length < 2) return raw;
            return splits[2];
        }

        public abstract void DanmuMessageHandlerAsync(string uid, string nickname, string content, string faceUrl);
        public abstract void GiftMessageHandlerAsync(string uid, string nickname, string faceUrl, string giftName, string giftId, int giftPrice, int giftCount);
        public virtual void LikeMessageHandlerAsync(string uid, string nickname, string faceUrl, int likeCount) { }

        public virtual void ParseGift(string uid, string giftName, string giftId, int giftPrice, int giftCount)
        {
            var user = UserManager.Instance.GetUser(uid);
            if (user != null)
            {
                var cast = this.giftConfigsDict.TryGet(giftName);
                if (cast != null)
                {
                    foreach (var item in cast.castConfigs)
                    {
                        var count = (int)(giftCount * item.castCount);
                        user.AddGift(item.cast, count, false);
                    }
                    DanmuCommandManager.Instance.NoticeGift(this.platform, giftName, uid, giftCount);
                }
                else
                {
                    if (this.未知礼物转换为礼物点)
                    {
                        user.AddGift($"礼物点_{giftName}", (int)(giftCount), false);
                        DanmuCommandManager.Instance.NoticeGift(this.platform, giftName, uid, giftCount);
                    }
                    else
                    {
                        user.AddGift(giftName, (int)(giftCount), false);
                        DanmuCommandManager.Instance.NoticeGift(this.platform, giftName, uid, giftCount);
                    }
                }
                user.SaveData();
            }
        }
    }
}
