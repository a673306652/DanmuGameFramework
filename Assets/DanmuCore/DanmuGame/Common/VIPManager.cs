using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;

namespace DanMuGame
{
    public class VIPManager : Manager<VIPManager>
    {
        [System.Serializable]
        public class VIPConfig
        {
            public int level;
            public int exp;
            public Color danmuColor;
            public Color nameColor;
        }
        public List<VIPConfig> vipConfigs = new List<VIPConfig>();


        public VIPConfig GetVipConfig(string id)
        {
            var user = UserManager.Instance.GetUser(id); 
            if (user != null)
            {
                var exp = user.Get<int>("exp");
                for (int i = 0; i < this.vipConfigs.Count; i++)
                {
                    var config = this.vipConfigs[i];
                    if (exp > config.exp) continue;
                    return config;
                }
                return this.vipConfigs.GetLast();
            }
            return this.vipConfigs[0];
        }
    }
}
