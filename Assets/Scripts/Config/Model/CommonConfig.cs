namespace Modules.Config.Model
{
    using System;
    using UnityEngine;
    [CreateAssetMenu(menuName = "Config/Common")]
    [Serializable]
    public class CommonConfig : ScriptableObject
    {
        [Header("topon_android_id")] public string TopOnAndroidId;
        [Header("topon_ios_id")] public string TopOnIosId;
        [Header("topon_android_key")] public string TopOnAndroidKey;
        [Header("topon_ios_key")] public string TopOnIosKey;
    }
}