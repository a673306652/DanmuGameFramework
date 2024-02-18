using System.Collections;
using System.Collections.Generic;
using Modules.Patterns;
using UnityEngine;

namespace DanMuGame
{
    public class DanmuCommandManager : Singleton<DanmuCommandManager>
    {
        public delegate bool DanmuCommand(string id, string content, int count = 1);
        public delegate int GiftCastCommand(string id, string cast, int total, string origin = "");
        public delegate void GiftNoticeCommand(string id, string giftName, int count, string origin = "");

        private Dictionary<string, DanmuCommand> dict = new Dictionary<string, DanmuCommand>();
        private Dictionary<string, DanmuCommand> dict2 = new Dictionary<string, DanmuCommand>();
        private Dictionary<string, DanmuCommand> gmCmdDict = new Dictionary<string, DanmuCommand>();
        private Dictionary<string, DanmuCommand> gmCmdDict2 = new Dictionary<string, DanmuCommand>();

        private Dictionary<string, GiftCastCommand> giftActionDict = new Dictionary<string, GiftCastCommand>();
        private Dictionary<string, GiftNoticeCommand> giftNoticeActionDict = new Dictionary<string, GiftNoticeCommand>();

        public DanmuCommand talkCommand;

        public void NoticeGift(string platform, string giftName, string id, int count, string origin = "")
        {
            var user = UserManager.Instance.GetUser(id);
            if (user == null) return;

            if (this.giftNoticeActionDict.ContainsKey(giftName))
                this.giftNoticeActionDict[giftName]?.Invoke(id, giftName, count, origin);
        }

        /// <summary>
        /// 这里需要传入礼物名
        /// </summary>
        public void AddGiftNotice(string giftName, GiftNoticeCommand action)
        {
            if (this.giftNoticeActionDict.ContainsKey(giftName))
            {
                Debug.LogError("重复注册");
                return;
            }
            this.giftNoticeActionDict.Add(giftName, action);
        }

        public void ParseCommand(string id, string cmd, int count = 1)
        {
            var user = UserManager.Instance.GetUser(id);
            if (user == null) return;

            var isGm = UserManager.Instance.IsGM(id);
            if (isGm)
            {
                if (this.gmCmdDict.ContainsKey(cmd))
                {
                    this.gmCmdDict[cmd].Invoke(id, cmd, count);
                    return;
                }

                foreach (var item in this.gmCmdDict2)
                {
                    if (cmd.Contains(item.Key))
                    {
                        this.gmCmdDict2[item.Key].Invoke(id, cmd, count);
                        return;
                    }
                }
                //GM的指令，都不要显示气泡框
            }

            if (this.dict.ContainsKey(cmd))
            {
                var boo = this.dict[cmd].Invoke(id, cmd, count);
                if (!boo) return;
            }
            foreach (var item in this.dict2)
            {
                if (cmd.Contains(item.Key))
                {
                    var boo = this.dict2[item.Key].Invoke(id, cmd, count);
                    if (!boo) return;
                }
            }
            this.talkCommand?.Invoke(id, cmd, count);
        }

        public void AddCommand(string key, DanmuCommand cmd, bool gmCmd = false)
        {
            if (gmCmd)
            {
                if (this.gmCmdDict.ContainsKey(key)) return;
                this.gmCmdDict[key] = cmd;
            }
            else
            {
                if (this.dict.ContainsKey(key)) return;
                this.dict[key] = cmd;
            }
        }

        public void AddLikeCommand(DanmuCommand cmd, bool gmCmd = false)
        {
            AddCommand("点赞", cmd, gmCmd);
        }

        /// <summary>
        /// 这里传入礼物投射
        /// </summary>
        public void AddGiftCastCommand(string cast, GiftCastCommand action)
        {
            if (this.giftActionDict.ContainsKey(cast))
            {
                Debug.LogError("重复注册");
                return;
            }
            this.giftActionDict.Add(cast, action);
        }

        public void AddContainsCommand(string key, DanmuCommand cmd, bool gmCmd = false)
        {
            if (gmCmd)
            {
                if (this.gmCmdDict2.ContainsKey(key)) return;
                this.gmCmdDict2[key] = cmd;
            }
            else
            {
                if (this.dict2.ContainsKey(key)) return;
                this.dict2[key] = cmd;
            }
        }

        public void RemoveCommand(string key)
        {
            this.dict.Remove(key);
        }

        /// <summary>
        /// 在MainLoop里面更新
        /// </summary>
        /// <param name="id"></param>
        public void UpdateGift(string id)
        {
            var user = UserManager.Instance.GetUser(id);
            var dirty = false;
            foreach (var item in this.giftActionDict)
            {
                if (item.Key == "礼物点")
                {
                    foreach (var key in new List<string>(user.stats.gifts.Keys))
                    {
                        var s = key.Split('_');
                        if (!s[0].Equals("礼物点")) continue;
                        var result = item.Value.Invoke(id, key, user.stats.gifts[key], s.Length > 1 ? s[1] : "");
                        user.stats.gifts[key] -= result;
                        dirty |= result > 0;
                    }
                }
                else
                {
                    var gift = user.stats.gifts.TryGet(item.Key);
                    if (gift > 0)
                    {
                        var result = item.Value.Invoke(id, item.Key, gift);
                        user.stats.gifts[item.Key] -= result;
                        dirty |= result > 0;
                    }
                }
            }

            if (dirty)
            {
                user.SaveData();
            }
        }

    }
}

