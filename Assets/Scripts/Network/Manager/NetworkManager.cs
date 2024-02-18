using System;
using System.Collections.Generic;
using BestHTTP;
using Modules.Config;
using Modules.Patterns;
using SimpleJSON;
using UnityEngine;

namespace Modules.Network.Manager
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        private static volatile NetworkManager instance;

        private Dictionary<string, HTTPRequest> httpRequestDic = new Dictionary<string, HTTPRequest>();
        private Dictionary<string, string> httpHeaderDic;
        private long lastRequestId = 0;

        public NetworkManager()
        {
            HTTPManager.MaxConnectionPerServer = NetworkConfigLoader.Instance.MaxConnectionPerServer;
            HTTPManager.RequestTimeout = new TimeSpan(0, 0, NetworkConfigLoader.Instance.RequestTimeout);
            HTTPManager.ConnectTimeout = new TimeSpan(0, 0, NetworkConfigLoader.Instance.ConnectionTimeout);
        }

        /// <summary>
        /// 配置请求头
        /// </summary>
        /// <param name="header"></param>
        public void ConfigHTTPHeader(Dictionary<string, string> header)
        {
            if (header != null)
            {
                httpHeaderDic = header;
            }
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public string PostRequest(Uri uri, string body, OnSuccess onSuccess, OnFail onFail = null)
        {
            lastRequestId++;
            string tag = lastRequestId.ToString();
            HTTPRequest request = new HTTPRequest(uri, HTTPMethods.Post, (req, resp) =>
            {
                HandleTextCallback(req, resp, (string result) =>
                {
                    onSuccess?.Invoke(result);
                }, onFail);
                RemoveRequest(tag);
            })
            {
                RawData = System.Text.Encoding.UTF8.GetBytes(body)
            };
            SendRequest(request, tag);
            return tag;
        }
        /// <summary>
        /// Get类型请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public string GetRequest(Uri uri, OnSuccess onSuccess, OnFail onFail = null)
        {
            lastRequestId++;
            string tag = lastRequestId.ToString();
            HTTPRequest request = new HTTPRequest(uri, (req, resp) =>
            {
                HandleTextCallback(req, resp, (string result) =>
                {
                    onSuccess?.Invoke(result);
                }, onFail);
                RemoveRequest(tag);
            });
            SendRequest(request, tag);
            return tag;
        }

        public void AbortAll()
        {
            foreach (var request in httpRequestDic.Values)
            {
                request.Abort();
            }
            httpRequestDic.Clear();
        }

        void ConfigDefaultHeader()
        {

        }

        void AbortRequest(string tag)
        {
            httpRequestDic.TryGetValue(tag, out HTTPRequest request);
            if (request != null)
            {
                request.Abort();
                httpRequestDic.Remove(tag);
            }
        }

        void AddRequest(string tag, HTTPRequest request)
        {
            if (httpRequestDic.ContainsKey(tag))
            {
                AbortRequest(tag);
            }
            httpRequestDic.Add(tag, request);
        }

        void HandleTextCallback(HTTPRequest request, HTTPResponse response, OnSuccess successCallback, OnFail failCallback)
        {
            using(response)
            {
                DebugExt.Network("request.Uri :  " + request.Uri.ToString());
                if (request.State == HTTPRequestStates.Finished && response.IsSuccess)
                {
                    string _ResponseText = response.DataAsText;
                    DebugExt.Network("request succeed with response : " + _ResponseText);
                    successCallback?.Invoke(_ResponseText);
                }
                else if (request.State == HTTPRequestStates.Finished && !response.IsSuccess)
                {
                    DebugExt.Network("request finished but fail");
                    HandleFailRequest(request, response, failCallback);
                }
                else
                {
                    // respone will be null
                    HandleFailRequest(request, response, failCallback);
                }
            }
        }

        /// <summary>
        /// 对失败请求的回调
        /// </summary>
        /// <param name="request"></param>
        /// <param name="failCallback"></param>
        void HandleFailRequest(HTTPRequest request, HTTPResponse response, OnFail failCallback)
        {
            try
            {
                ErrorState state = ErrorState.Unknown;
                int errorCode = response != null ? response.StatusCode : 0;
                string errorMsg = response != null ? response.Message : string.Empty;
                switch (request.State)
                {
                    case HTTPRequestStates.Error:
                        state = ErrorState.Error;
                        errorMsg = "Request Finished with Error! " + request.Exception != null ? request.Exception.Message : "No Exception";
                        break;

                        // The request aborted, initiated by the user.
                    case HTTPRequestStates.Aborted:
                        state = ErrorState.Aborted;
                        errorMsg = "Request Aborted!";
                        break;

                        // Ceonnecting to the server timed out.
                    case HTTPRequestStates.ConnectionTimedOut:
                        state = ErrorState.ConnectionTimedOut;
                        errorMsg = "Connection Timed Out!";
                        break;

                        // The request didn't finished in the given time.
                    case HTTPRequestStates.TimedOut:
                        state = ErrorState.TimedOut;
                        errorMsg = "Processing the request Timed Out!";
                        break;
                    case HTTPRequestStates.Finished:
                        ErrorMessageEntity errorMessageEntity = new ErrorMessageEntity();
                        try
                        {
                            DebugExt.Network("error state : " + response.DataAsText.ToString());
                            var jsonObject = JSON.Parse(response.DataAsText).AsObject;
                            // JsonData jsonData = JsonMapper.ToObject(response.DataAsText);
                            string messageStr = jsonObject ["message"].ToString().Replace(" ", "");;
                            string codeStr = jsonObject ["code"].ToString();
                            string url = request.Uri.ToString();
                            string method = request.MethodType.ToString();
                            DebugExt.Network("error state : " + response.DataAsText.ToString());
                            // UmengHelper.OnEvent(UmengEventName.NetWork_Error_Reason,  "reason", url+"|" + method+"|"+codeStr + "|" + messageStr.Trim());
                            // Debugger.LogTest("result:"+(url+"|" + method , codeStr + " | " + messageStr));
                            // errorMessageEntity.message = ToastConst.NETWORK_ERROR;
                        }
                        catch (Exception ex)
                        {
                            // errorMessageEntity.message = ToastConst.NETWORK_ERROR;
                        }
                        state = ErrorState.Unknown;
                        errorMsg = errorMessageEntity.message;
                        break;
                    default:
                        state = ErrorState.Unknown;
                        errorMsg = response.Message;
                        break;
                }

                NetworkError error = new NetworkError
                {
                    code = errorCode,
                    message = errorMsg,
                    state = state
                };

                DebugExt.Network("HandleFailRequest with error state : " + error.state.ToString() +
                    "with error msg : " + error.message);
                failCallback?.Invoke(error);
            }
            catch (System.Exception e)
            {
                DebugExt.Network("HandleFailRequest exception  : " + e.Message);
            }

        }

        void RemoveRequest(string tag)
        {
            httpRequestDic.TryGetValue(tag, out HTTPRequest request);
            if (request != null)
            {
                httpRequestDic.Remove(tag);
            }
        }

        private void SendRequest(HTTPRequest request, string tag)
        {
            AddHttpHeader(request);
            AddRequest(tag, request);
            request.Send();
            DebugExt.Network("url: " + request.Uri.ToString());
            if (request.MethodType == HTTPMethods.Post)
            {
                DebugExt.Network("Request Body : " + System.Text.Encoding.UTF8.GetString(request.RawData));
            }
        }

        private void AddHttpHeader(HTTPRequest request)
        {
            foreach (KeyValuePair<string, string> kv in httpHeaderDic)
            {
                request.AddHeader(kv.Key, kv.Value);
            }

        }
    }
}