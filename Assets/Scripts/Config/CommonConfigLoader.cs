namespace Modules.Config
{
    using System.IO;
    using System;
    using Modules.Config.Model;
    using Modules.Patterns;
    using UnityEngine;

    public class CommonConfigLoader : Singleton<CommonConfigLoader>
    {
        private readonly static string CONFIG_DIRECTORY = "Config";
        private CommonConfig configModel;

        public CommonConfigLoader()
        {
            configModel = ScriptableObject.CreateInstance<CommonConfig>();
            string configPath = Path.Combine(CONFIG_DIRECTORY, configModel.GetType().Name);
            configModel = Resources.Load<CommonConfig>(configPath);
            if (configModel == null)
            {
                throw new NullReferenceException();
            }
        }

        public string TopOnId
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)return configModel.TopOnAndroidId;
                if (Application.platform == RuntimePlatform.IPhonePlayer)return configModel.TopOnIosId;
                return string.Empty;
            }
        }

        public string TopOnKey
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)return configModel.TopOnAndroidKey;
                if (Application.platform == RuntimePlatform.IPhonePlayer)return configModel.TopOnIosKey;
                return string.Empty;
            }
        }
    }

}