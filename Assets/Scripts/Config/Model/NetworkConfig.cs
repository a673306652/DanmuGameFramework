namespace Modules.Config.Model
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Config/Network")]
    [Serializable]
    public class NetworkConfig : ScriptableObject
    {
        [Header("测试服地址")]
        public string DevServerDomain;

        [Header("正式服地址")]
        public string ProServerDomain;

        [Header("正式服备份地址")]
        public string[] ProServerDomainBackUp;

        [Header("客户端一次最多能激活的tcp连接数")]
        public byte MaxConnectionPerServer;

        [Header("请求超时时间(秒)")]
        public int RequestTimeout;

        [Header("连接超时时间(秒)")]
        public int ConnectionTimeout;

        // 由自己定义，和服务器协商即可
        [Header("是否使用测试服")]
        public bool IsDevMode;

        [Header("应用内部代号")]
        public string ApplicationId;

        [Header("客户端解密服务器数据密钥")]
        public string SecretKey;
    }
}
