namespace Modules.Config
{
    using System.IO;
    using System;
    using Modules.Config.Model;
    using Modules.Patterns;
    using UnityEngine;

    public class NetworkConfigLoader : Singleton<NetworkConfigLoader>
    {
        private readonly static string CONFIG_DIRECTORY = "Config";
        private NetworkConfig configModel;
        public NetworkConfigLoader()
        {
            configModel = ScriptableObject.CreateInstance<NetworkConfig>();
            string configPath = Path.Combine(CONFIG_DIRECTORY, configModel.GetType().Name);
            configModel = Resources.Load<NetworkConfig>(configPath);
            if (configModel == null)
            {
                Debug.LogError("请创建相关的配置信息");
                throw new NullReferenceException();
            }
        }

        public string DevSerDomain
        {
            get
            {
                return configModel.DevServerDomain;
            }
        }

        public bool IsProdServer
        {
            get
            {
                return !configModel.IsDevMode;
            }
        }

        // public string lastInstallVersion { get; set; }

        /// <summary>
        /// 获取当前服务器域名
        /// </summary>
        public string GetServerDomain()
        {
            if (IsProdServer)
            {
                return ProServerDomain;
            }
            return DevSerDomain;
        }

        public string [] GetBackUpServerDomain()
        {
            if (IsProdServer)
            {
                return BackUpServerDomain;
            }
            return new string [] { DevSerDomain };
        }

        private string [] BackUpServerDomain
        {
            get
            {
                return configModel.ProServerDomainBackUp;
            }
        }

        private string ProServerDomain
        {
            get
            {
                return configModel.ProServerDomain;
            }
        }

        public byte MaxConnectionPerServer
        {
            get
            {
                return configModel.MaxConnectionPerServer;
            }
        }

        public int RequestTimeout
        {
            get
            {
                return configModel.RequestTimeout;
            }
        }

        public int ConnectionTimeout
        {
            get
            {
                return configModel.ConnectionTimeout;
            }
        }

        public string ApplicationId
        {
            get
            {
                return configModel.ApplicationId;
            }
        }

        public string SecretKey
        {
            get
            {
                return configModel.SecretKey;
            }
        }
    }

}