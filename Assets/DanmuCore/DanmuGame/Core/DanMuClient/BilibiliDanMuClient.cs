using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using Daan;
using System;
using System.Text;
using System.Threading;
using NativeWebSocket;
using OpenBLive.Runtime;
using OpenBLive.Runtime.Data;
using OpenBLive.Runtime.Utilities;
using DanMuGame;
using Task = System.Threading.Tasks.Task;
using Random = UnityEngine.Random;

namespace DanMuGame
{

    public class BilibiliDanMuClient : BaseDanmuClient
    {
        public string 身份码;
        public string appId;
        public string accessKeySecret;
        public string accessKeyId;

        public string fansName;

        private string gameId;
        private Timer reconnectTimer;


        private WebSocketBLiveClient m_WebSocketBLiveClient;

        private Timer officeGiftHeartTimer;

        protected virtual void Update()
        {
            if (!this.readyToConnect) return;
#if !UNITY_WEBGL || UNITY_EDITOR
            try
            {
                if (m_WebSocketBLiveClient is { ws: { State: WebSocketState.Open } })
                {
                    m_WebSocketBLiveClient.ws.DispatchMessageQueue();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
#endif
            this.UpdateReconnect();
        }

        protected override void Awake()
        {
            base.Awake();
            Message.Main.AddListener(Message.SystemHints.Connecting, async (obj) =>
            {
                this.reconnectTimer = Timer.Register(this.reconnectTime, () =>
                {
                    this.reconnectTimer = null;
                });
                var sucess = await this.Connect();
                if (sucess)
                {
                    Message.Main.Broadcast(Message.SystemHints.ConnectionSuccess);
                    Message.Main.Broadcast(Message.CommonCmd.GameStart);
                }
                else
                {
                    Message.Main.Broadcast(Message.SystemHints.ConnectionError);
                }
                this.readyToConnect = true;
            });
        }

        async void UpdateReconnect()
        {
            if (this.m_WebSocketBLiveClient == null || this.m_WebSocketBLiveClient.ws == null || this.m_WebSocketBLiveClient.ws.State == WebSocketState.Closed)
            {
                this.连接成功 = false;
                if (this.reconnectTimer == null)
                {
                    Debug.Log("掉线了，重新连");
                    this.m_WebSocketBLiveClient?.Disconnect();
                    this.reconnectTimer = Timer.Register(this.reconnectTime, () =>
                    {
                        this.reconnectTimer = null;
                    });
                    var boo = await this.ConnectToOffcial();
                    if (!boo)
                    {
                        Debug.Log($"重连失败, {this.reconnectTime} 秒后尝试重连");
                    }
                }
            }
            else
            {
                this.连接成功 = true;
            }
        }

        public void DisconnectOfficeGift()
        {
            this.officeGiftHeartTimer?.Cancel();
            this.officeGiftHeartTimer = null;
            BApi.EndInteractivePlay(this.appId, this.gameId);
        }

        async Task<bool> Connect()
        {
            if (!this.连接成功)
            {
                return await this.ConnectToOffcial();
            }
            return false;
        }

        public string GetConnectState()
        {
            if (this.m_WebSocketBLiveClient == null || this.m_WebSocketBLiveClient.ws == null || this.m_WebSocketBLiveClient.ws.State == WebSocketState.Closed)
            {
                return "连接失败";
            }
            else
            {
                return "连接成功";
            }
        }

        public HisaoTask linkTask;
        public HisaoTask closeLinkTask;
        
        [HideInInspector]
        public string resultCode;
        public virtual async Task<bool> ConnectToOffcial()
        {
            Debug.Log("开始连接弹幕服务器");
            //测试环境的域名现在不可用，默认开启正式环境
            BApi.isTestEnv = false;
            //测试的密钥
            SignUtility.accessKeySecret = this.accessKeySecret;
            //测试的ID
            SignUtility.accessKeyId = this.accessKeyId;
            
            if (linkTask!= null)
            {
                if (closeLinkTask == null && !连接成功)
                {
                    
                    closeLinkTask = Main.Instance.Exec((a) =>
                    {
                        if (!连接成功)
                        {
                            //表示已经触发重连，但是还没重连成功，这里需要调用关闭心跳的api
                            DisconnectOfficeGift();
                            Debug.Log("关闭了连接");
                        }
                        
                        closeLinkTask = null;
                    }, 60);
                }
                return false;
            }
            linkTask = Main.Instance.Exec((a) =>
            {
                linkTask = null;
            },120);

            var result = await BApi.StartInteractivePlay(this.身份码, this.appId);
            var gameIdResObj = JsonConvert.DeserializeObject<AppStartInfo>(result);
            if (gameIdResObj != null)
            {
                this.resultCode = gameIdResObj.Message;
            }
            else
            {
                this.resultCode = result;
            }
            try
            {
                m_WebSocketBLiveClient = new WebSocketBLiveClient(gameIdResObj);

                m_WebSocketBLiveClient.OnDanmaku += BLiveDanmakuMessageHandlerAsync;
                m_WebSocketBLiveClient.OnGift += BLiveGiftMessageHandlerAsync;
                m_WebSocketBLiveClient.OnGuardBuy += BLiveGuardBuyMessageHandlerAsync;

                m_WebSocketBLiveClient.Connect();
            }
            catch (Exception e)
            {

                return false;
            }

            if (!string.IsNullOrEmpty(result))
            {
                var jobject = JObject.Parse(result);
                this.gameId = jobject["data"]["game_info"]["game_id"].ToString();
                Debug.Log("B站官方SDK连接成功:" + result);

                this.officeGiftHeartTimer = Timer.Register(30, async () =>
                {
                    var str = await BApi.HeartBeatInteractivePlay(this.gameId);
                    if (string.IsNullOrEmpty(str))
                    {
                        var heartBeatResult = JObject.Parse(str);
                        if (heartBeatResult == null || heartBeatResult["message"].ToString() != "ok")
                        {
                            Debug.LogError(str);
                            m_WebSocketBLiveClient.Disconnect();
                            m_WebSocketBLiveClient.Dispose();
                            m_WebSocketBLiveClient = null;
                            this.officeGiftHeartTimer.Cancel();
                            await this.ConnectToOffcial();
                        }
                    }
 
                }, null, true);
            }
            return true;
        }

        public bool 本局激活 = false;
        public bool 检查许可证(string uid, string content)
        {
            foreach (var item in UserManager.Instance.gmList)
            {
                if (item.uid == uid)
                {
                    if (content.Equals("永久激活"))
                    {
                        PlayerPrefs.SetInt("永久激活", 1);
                    }

                    if (content.Equals("本局激活"))
                    {
                        this.本局激活 = true;
                    }

                    if (content.Equals("取消激活"))
                    {
                        PlayerPrefs.SetInt("永久激活", -1);
                        Application.Quit();
                    }
                    break;
                }
            }
            var 永久激活 = PlayerPrefs.GetInt("永久激活", -1) >= 0;
            if (永久激活) this.本局激活 = 永久激活;
            return this.本局激活;
        }

        public override void DanmuMessageHandlerAsync(string uid, string nickname, string content, string faceUrl)
        {
         
            var user = UserManager.Instance.UpdateUserInfo(this.platform, this.roomId, uid, nickname, faceUrl);

            //@这里补一个VIP
            //UserManager.Instance.SetVip(user, this.GetVipLevel(fansMedalName, guardLevel));

            UserManager.Instance.UpdateActivePlayerList(this.platform, this.roomId, uid);

            //把这个下载假用户挪到一个单独的Client去
            //FakeUserManager.Instance.AddUser(nickname, faceUrl);

            DanmuCommandManager.Instance.ParseCommand(UserManager.GetUserKey(user.info), content);
        }

        public override void GiftMessageHandlerAsync(string uid, string nickname, string faceUrl, string giftName, string giftId, int giftPrice, int giftCount)
        {
     
            var user = UserManager.Instance.UpdateUserInfo(this.platform, this.roomId, uid, nickname, faceUrl);

            //@这里补一个VIP
            //UserManager.Instance.SetVip(user, this.GetVipLevel(message.fansMedalName, message.guardLevel));

            UserManager.Instance.UpdateActivePlayerList(this.platform, this.roomId, uid);
            FakeUserManager.Instance.AddUser(uid,faceUrl, faceUrl);

            this.ParseGift(UserManager.GetUserKey(user.info), giftName, giftId, giftPrice, giftCount);
        }

        void BLiveDanmakuMessageHandlerAsync(Dm message)
        {
            this.DanmuMessageHandlerAsync(message.uid.ToString(), message.userName, message.msg, message.userFace);
        }

        void BLiveGiftMessageHandlerAsync(SendGift message)
        {
            this.GiftMessageHandlerAsync(message.uid.ToString(), message.userName, message.userFace, message.giftName, message.giftId.ToString(), (int)message.price, (int)message.giftNum);
        }

        void BLiveGuardBuyMessageHandlerAsync(Guard message)
        {
             //上舰消息，没写到handler，自己整吧
        }


        /// <summary>
        /// 每个平台都不一样，暂时没用
        /// </summary>
        /// <param name="hz"></param>
        /// <param name="guardLevel"></param>
        /// <returns></returns>
        public long GetVipLevel(string hz, long guardLevel)
        {
            if (guardLevel > 0)
            {
                return 5 - guardLevel;
            }
            else
            {
                if (!string.IsNullOrEmpty(hz) && hz.Equals(this.fansName))
                {
                    return 1;
                }
            }
            return 0;
        }

        private void OnApplicationQuit()
        {
            this.m_WebSocketBLiveClient?.Dispose();
            this.DisconnectOfficeGift();
            Debug.Log("游戏退出");
        }
    }
}