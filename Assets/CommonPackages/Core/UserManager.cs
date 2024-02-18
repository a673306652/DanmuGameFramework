using System;
using System.Collections.Generic;
using System.ComponentModel;
using Modules.Patterns;
using UnityEngine;

namespace DanMuGame
{
    [System.Serializable]
    public class UserBaseInfo
    {
        public string platform;
        public long rid;
        public string uid;

        public override bool Equals(object obj)
        {
            var other = obj as UserBaseInfo;
            if (other != null)
            {
                return this.platform == other.platform && this.rid == other.rid && this.uid == other.uid;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.platform.GetHashCode() * 10000 + this.rid.GetHashCode() * 100 + this.uid.GetHashCode();
        }
    }

    [System.Serializable]
    public class UserStats
    {
        public Dictionary<string, int> gifts = new Dictionary<string, int>(); // 礼物统计
        public Dictionary<string, int> commands = new Dictionary<string, int>(); // 指令统计
        public int likes; // 点赞统计
    }

    public class DanMuUser
    {
        public string uid;
        public UserBaseInfo info;
        public UserStats stats;
        public string nickname;
        public Sprite icon;
        public string faceUrl;

        public override bool Equals(object obj)
        {
            var other = obj as DanMuUser;
            if (other != null)
            {
                return other.info == this.info;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return this.info.GetHashCode();
        }

        #region 存储数据

        private Dictionary<string, object> data = new Dictionary<string, object>();
        private Dictionary<string, string> rawData = new Dictionary<string, string>();

        public DateTime GetDateTime(string key, DateTime defaultValue = default)
        {
            var str = this.Get<string>(key, defaultValue.ToNormalString());
            return Convert.ToDateTime(str);
        }

        public T Get<T>(string key, T defaultValue = default) where T : IComparable
        {
            if (!this.data.ContainsKey(key))
            {
                if (!this.rawData.ContainsKey(key))
                {
                    this.data[key] = defaultValue;
                    this.SaveData();
                    return defaultValue;
                }
                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));//TWeight是自定义的一个泛型类型
                    this.data[key] = (T)converter.ConvertTo(this.rawData[key], typeof(T));
                }
                catch (Exception e)
                {
                    Debug.LogError($"{key}的类型不对啊,原始数据是{this.rawData[key]}");
                    return defaultValue;
                }
            }
            return (T)this.data[key];
        }

        public void Set<T>(string key, Func<T, T> func, T d = default) where T : IComparable
        {
            var value = this.Get<T>(key, d);
            value = func.Invoke(value);
            this.Set(key, value);
        }

        public void Set(string key, object value)
        {
            if (!this.data.ContainsKey(key))
            {
                this.data.Add(key, value);
                return;
            }
            this.data[key] = value;
        }

        public void AddGift(string giftName, int count, bool needSave = true)
        {
            if (!this.stats.gifts.ContainsKey(giftName))
            {
                this.stats.gifts.Add(giftName, 0);
            }
            this.stats.gifts[giftName] += count;
            if (needSave)
            {
                this.SaveData();
            }
        }

        public void SaveData(bool rightNow = false)
        {
            List<string> list = new List<string>();
            var temp = new Dictionary<string, string>(this.rawData);
            foreach (var item in this.data)
            {
                if (!temp.ContainsKey(item.Key))
                {
                    temp.Add(item.Key, "");
                }
                temp[item.Key] = Convert.ToString(item.Value);
            }

            foreach (var item in temp)
            {
                if (item.Key != "Gifts" && item.Key != "Gift")
                {
                    var paramStr = $"{item.Key}#{item.Value}";
                    list.Add(paramStr);
                }
            }

            if (this.stats.gifts.Count > 0)
            {
                var giftStr = "";
                foreach (var item in this.stats.gifts)
                {
                    if (item.Value == 0) continue;
                    giftStr += $"{item.Key}:{item.Value.ToString()}-";
                }
                list.Add($"Gifts#{giftStr}");
            }
            var key = UserManager.Instance.GetSaveKey(this.info);
            var value = string.Join("|", list);

            PlayerPrefs.SetString(key, value);
        }

        public void GetData(bool create = true)
        {
            var str = PlayerPrefs.GetString($"{UserManager.Instance.GetSaveKey(this.info)}");
            if (string.IsNullOrEmpty(str))
            {
                if (create)
                {
                    //新玩家情况
                    UserManager.Instance.OnNewUserRegister?.Invoke(this);
                }
            }
            else
            {
                var param = str.Split('|');
                foreach (var item in param)
                {
                    var kv = item.Split('#');
                    if (kv.Length == 2)
                    {
                        if (!this.rawData.ContainsKey(kv[0]))
                        {
                            this.rawData.Add(kv[0], kv[1]);
                        }
                    }
                }

                if (this.rawData.ContainsKey("Gifts"))
                {
                    var gift = this.rawData["Gifts"].Split('-');
                    foreach (var item in gift)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var kv = item.Split(':');
                            var value = kv[1].ToInt();
                            if (value == 0) continue;
                            this.stats.gifts.Add(kv[0], value);
                        }
                    }
                }
            }
        }

        #endregion
    }

    public class UserManager : MonoSingleton<UserManager>
    {
        public float 卸载时长;
        [Header("填写游戏名后才会保存玩家数据")]
        public string gameName = "";
        [HideInInspector] public Dictionary<string, DanMuUser> allUserDict = new Dictionary<string, DanMuUser>();
        /// <summary>
        /// 活跃玩家列表，无论哪个平台，发送弹幕和礼物都能提高活跃
        /// </summary>
        [HideInInspector] public List<string> activeUserList = new List<string>();
        [HideInInspector] public Action<DanMuUser, TimeSpan> OnUserActive;
        [HideInInspector] public Action<DanMuUser> OnNextDay;
        [HideInInspector] public Action<DanMuUser> OnNewUserRegister;


        private void Awake()
        {
            //每60秒，卸载一次离线过久的玩家
            Timer.Register(60, () =>
             {
                 this.UnloacdInactiveUser();
             }, null, true);
        }

        public bool ContainsUser(string id)
        {
            return allUserDict.ContainsKey(id);
        }

        /// <summary>
        /// @Daan，之后这里直接走远程配置
        /// </summary>
        public List<UserBaseInfo> gmList = new List<UserBaseInfo>();
        public void AddGM(UserBaseInfo info)
        {
            if (!this.gmList.Contains(info)) this.gmList.Add(info);
        }

        public bool IsGM(string id)
        {
            var info = UserManager.Instance.GetUserBaseInfo(id);
            foreach (var item in this.gmList)
            {
                var uid = item.uid == info.uid;
                //没配平台信息的为全局管理者
                var platform = item.platform == info.platform || string.IsNullOrEmpty(info.platform);
                //没配房间信息的为全局管理者
                var room = item.rid == info.rid || info.rid == 0;
                if (uid && platform && room)
                {
                    return true;
                }
            }
            return false;
        }

        public UserBaseInfo GetUserBaseInfo(string id)
        {
            var str = id.Split('/');
            return new UserBaseInfo()
            {
                platform = str[0],
                rid = str[1].ToLong(),
                uid = str[2],
            };
        }

        public static string GetUserKey(UserBaseInfo info)
        {
            return $"{info.platform}/{info.rid}/{info.uid}";
        }

        public static string GetUserKey(string platform, long rid, string uid)
        {
            return $"{platform}/{rid}/{uid}";
        }

        public static string GetUserUID(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }
            var splits = key.Split('/');
            if (splits.Length < 2) return key;
            return splits[2];
        }

        public string GetSaveKey(UserBaseInfo info)
        {
            return this.GetSaveKey(info.platform, info.rid, info.uid);
        }

        public string GetSaveKey(string platform, long rid, string uid)
        {
            if (string.IsNullOrEmpty(this.gameName)) return null;
            return $"{this.gameName}/{GetUserKey(platform, rid, uid)}";
        }

        public DanMuUser GetUser(string id, bool onlyOnline = true)
        {
            if (string.IsNullOrEmpty(id)) return null;
            if (this.allUserDict.ContainsKey(id))
            {
                return this.allUserDict[id];
            }
            else
            {
                if (!onlyOnline)
                {
                    var info = this.GetUserBaseInfo(id);
                    //@Daan，这一步是一定会创建一个User的，看看会不会有问题
                    var user = this.GetLocalUser(info.platform, info.rid, info.uid);
                    if (user != null)
                    {
                        return user;
                    }
                }
            }
            Debug.LogWarning("尝试访问一个不存在的uid，排查代码问题，或是UserManager和Main脚本的调用顺序问题");
            return null;
        }

        public DanMuUser GetUser(UserBaseInfo info, bool onlyOnline = true)
        {
            return this.GetUser(GetUserKey(info), onlyOnline);
        }

        public List<DanMuUser> GetAllActiveUsers()
        {
            return this.activeUserList.ConvertAll(s => this.allUserDict[s]);
        }

        public List<string> GetAllKeys()
        {
            return new List<string>(this.allUserDict.Keys);
        }

        public List<DanMuUser> GetAllUsers()
        {
            return new List<DanMuUser>(this.allUserDict.Values);
        }

        public void UpdateActivePlayerList(string platform, long rid, string uid)
        {
            var key = GetUserKey(platform, rid, uid);
            if (string.IsNullOrEmpty(key)) return;
            if (this.activeUserList.Contains(key))
            {
                this.activeUserList.Remove(key);
            }
            var user = this.GetUser(key, true);
            //Debug.Log($"{user.nickname}最近活跃了，把他的活跃度置顶");
            this.activeUserList.Insert(0, key);
        }

        public DanMuUser UpdateUserInfo(string platform, long rid, string uid, string name, string faceUrl)
        {
            DanMuUser user = null;
            var key = GetUserKey(platform, rid, uid);
            if (string.IsNullOrEmpty(key)) return user;
            if (!this.allUserDict.ContainsKey(key))
            {
                user = new DanMuUser()
                {
                    uid = uid,
                    info = new UserBaseInfo()
                    {
                        uid = uid,
                        platform = platform,
                        rid = rid,
                    },
                    stats = new UserStats()
                    {
                        gifts = new Dictionary<string, int>(),
                        commands = new Dictionary<string, int>()
                    }
                };
                user = this.GetUserLocalData(user);
                this.allUserDict.Add(key, user);
                Debug.LogWarning($"新用户来到房间:{uid}");
            }

            user = this.allUserDict[key];
            user.nickname = name;
            var timeSpan = DateTime.Now - user.GetDateTime("lastActiveTime", DateTime.Now);
            UserManager.Instance.OnUserActive?.Invoke(user, timeSpan);
            user.Set("lastActiveTime", DateTime.Now.ToNormalString());

            var loginTime = user.GetDateTime("loginTime", DateTime.Now);
            var nextDay = DateTime.Now.Day != loginTime.Day;
            if (nextDay)
            {
                user.Set("loginTime", DateTime.Now.ToNormalString());
                this.OnNextDay?.Invoke(user);
            }

            //如果同一个UID的玩家头像发生过改变则下载一次
            if (!string.IsNullOrEmpty(faceUrl) && !string.Equals(user.faceUrl, faceUrl))
            {
                user.faceUrl = faceUrl;
                UpdateUserIcon(user, faceUrl); ;
            }
            return user;
        }

        private void UpdateUserIcon(DanMuUser user, string url)
        {
            var iconCache = DynamicAtlasManager.Instance.GetUserIcon(url);
            if (null != iconCache)
            {
                user.icon = iconCache;
                return;
            }
            DynamicAtlasManager.Instance.DownloadUserIcon(url, (sprite) =>
            {
                user.icon = sprite;
            });
        }

        public List<string> GetActivePlayerList()
        {
            return this.activeUserList;
        }

        DanMuUser GetUserLocalData(DanMuUser user)
        {
            if (string.IsNullOrEmpty(this.gameName))
            {
                Debug.LogWarning("没有填写游戏名，无法保存玩家数据");
                return user;
            }
            user.GetData();
            return user;
        }

        private DanMuUser GetLocalUser(string platform, long rid, string uid)
        {
            var key = GetUserKey(platform, rid, uid);

            if (string.IsNullOrEmpty(key)) return null;
            var user = new DanMuUser()
            {
                uid = uid,
                info = new UserBaseInfo()
                {
                    uid = uid,
                    platform = platform,
                    rid = rid,
                },
                stats = new UserStats()
                {
                    gifts = new Dictionary<string, int>(),
                    commands = new Dictionary<string, int>()
                }
            };
            user.GetData();
            return user;
        }

        public void SaveUserLocalData(DanMuUser user)
        {
            if (!string.IsNullOrEmpty(this.gameName))
            {
                user.SaveData();
            }
            else
            {
                Debug.LogWarning("没有填写游戏名，无法保存玩家数据");
            }
        }

        public void SaveAllUserLocalData()
        {
            foreach (var item in this.allUserDict.Values)
            {
                this.SaveUserLocalData(item);
            }
        }

        void UnloacdInactiveUser()
        {
            var time = DateTime.Now;
            var list = new List<string>();
            foreach (var item in this.allUserDict.Values)
            {
                var key = GetUserKey(item.info);
                if (!this.activeUserList.Contains(key))
                {
                    if ((time - item.GetDateTime("lastActiveTime")).TotalSeconds >= this.卸载时长)
                    {
                        list.Add(key);
                    }
                }
            }

            foreach (var item in list)
            {
                this.allUserDict[item].faceUrl = null;
                this.allUserDict[item].icon = null;
                this.allUserDict[item].Set("lastActiveTime", time.ToNormalString());
            }

            Debug.Log($"本次释放了{list.Count}个不活跃的玩家（只释放头像内存）");
        }

        /// <summary>
        /// 对于有些超长对局的游戏（甚至一次直播就等于一局的挂机类游戏），手动的释放一些用户会好一些
        /// </summary>
        /// <param name="id"></param>
        public void UnloadUser(string id)
        {
            this.SaveUserLocalData(this.GetUser(id));
            this.allUserDict.Remove(id);
            this.activeUserList.Remove(id);
        }
        /// <summary>
        /// 一局游戏结束以后最好手动释放一下
        /// </summary>
        public void UnloadAllUser()
        {
            this.SaveAllUserLocalData();
            this.activeUserList.Clear();
            this.allUserDict.Clear();
        }
    }
}
