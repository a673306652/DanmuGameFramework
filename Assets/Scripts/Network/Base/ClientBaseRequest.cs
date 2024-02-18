namespace Modules.Network
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Modules.Config;
    using Modules.Network.Manager;
    using UnityEngine;

    public delegate void RequestSuccess<RequestOutput>(RequestOutput output);
    public delegate void RequestFail(NetworkError error);

    public class ClientBaseRequest<Operation, Input, Output>
        where Operation : RequestOperation, new()
    where Input : RequestInput, new()
    where Output : RequestOutput, new()
    {
        public RequestAPI<Operation, Input, Output> RequestAPI;
        // API 域名连通性权重表
        private readonly static Dictionary<string, int> apiDomainConnDict = new Dictionary<string, int>();
        private bool isAddApiDomain = true;

        public ClientBaseRequest()
        {
            RequestAPI = new RequestAPI<Operation, Input, Output>();
        }

        public string Request(RequestSuccess<Output> onSuccess = null, RequestFail onFail = null)
        {
            if (RequestAPI == null)
            {
                throw new NullReferenceException();
            }
            string requestName = RequestAPI.operation.GetType().Name;
            RequestSuccess<Output> success = (Output output) =>
            {
                onSuccess?.Invoke(output);
            };

            RequestFail fail = (NetworkError error) =>
            {
                onFail?.Invoke(error);
            };

            if (RequestAPI.operation.Method == Method.Post)
            {
                return PostRequst(success, fail);
            }
            else
            {
                return GetRequst(success, fail);
            }
        }

        private string PostRequst(RequestSuccess<Output> onSuccess = null, RequestFail onFail = null)
        {

            RequestInput input = RequestAPI.input;
            string body = input.ToJson();
            Debug.Log("Request with body : " + body);

            Uri uri = new UriBuilder(Path()).Uri;
            bool isAddDomain = isAddApiDomain;
            OnSuccess successHandler = (string result) =>
            {
                Output output = new Output
                {
                    output = result
                };
                if (onSuccess != null)
                {
                    // 成功解析域名，域名增权
                    if (isAddDomain)
                    {
                        apiDomainConnDict [uri.Host] += 1;
                    }
                    onSuccess(output);
                }
            };

            OnFail failHandler = (NetworkError failError) =>
            {
                if (onFail != null)
                {
                    // 无法解析域名，域名降权
                    if (isAddDomain && failError.code == 0)
                    {
                        apiDomainConnDict [uri.Host] -= 1;
                    }
                    onFail(failError);
                }
            };

            return NetworkManager.Instance.PostRequest(uri, body, successHandler, failHandler);
        }

        private string GetRequst(RequestSuccess<Output> onSuccess = null, RequestFail onFail = null)
        {
            RequestInput input = RequestAPI.input;
            string body = input.ToJson();
            Debug.Log("Request with body : " + body);

            Uri uri = new Uri(Path(true));
            bool isAddDomain = isAddApiDomain;
            OnSuccess successHandler = (string result) =>
            {
                Output output = new Output
                {
                    output = result
                };
                if (onSuccess != null)
                {
                    // 成功解析域名，域名增权
                    if (isAddDomain)
                    {
                        apiDomainConnDict [uri.Host] += 1;
                    }
                    onSuccess(output);
                }

            };

            OnFail failHandler = (NetworkError failError) =>
            {

                if (onFail != null)
                {
                    // 无法解析域名，域名降权
                    if (isAddDomain && failError.code == 0)
                    {
                        apiDomainConnDict [uri.Host] -= 1;
                    }
                    onFail(failError);
                }

            };

            return NetworkManager.Instance.GetRequest(uri, successHandler, failHandler);
        }

        /// <summary>
        /// 是否添加token 请求头
        /// </summary>
        /// <param name="isAddToken"></param>
        /// <param name="token"></param>
        public void ConfigHttpHeader(bool isAddToken, string token = "")
        {
            Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            };
            if (isAddToken && token != string.Empty)
            {
                header.Add("Authorization", "Bearer " + token);
            }
            NetworkManager.Instance.ConfigHTTPHeader(header);
        }

        /// <summary>
        /// 是否用预定的域名
        /// </summary>
        /// <param name="isAddDomain"></param>
        public void ConfigHttpDomain(bool isAddDomain = true)
        {
            isAddApiDomain = isAddDomain;
        }

        private bool isGetAddHttp = true;
        /// <summary>
        /// 是否Get请求增加http或者增加https,默认增加HTTP
        /// </summary>
        /// <param name="isHttp"></param>
        public void IsHttpForGetRequest(bool isHttp = true)
        {
            isGetAddHttp = isHttp;
        }

        private string Path(bool isGetRequest = false)
        {
            if (isGetRequest)
            {
                if (!isAddApiDomain)
                {
                    return RequestAPI.operation.Route;
                }
                else
                {
                    string protocol = isGetAddHttp? "http://": "https://";
                    string fullRequestURL = protocol + ApiDomain() + RequestAPI.operation.Route;
                    return fullRequestURL;
                }
            }
            return isAddApiDomain? ApiDomain() + RequestAPI.operation.Route : RequestAPI.operation.Route;
        }

        private string ApiDomain()
        {
            if (apiDomainConnDict.Count < 1)
            {
                apiDomainConnDict [NetworkConfigLoader.Instance.GetServerDomain()] = 0;
                var domains = NetworkConfigLoader.Instance.GetBackUpServerDomain();
                foreach (var d in domains)
                {
                    apiDomainConnDict [d] = 0;
                }
            }
            // 权重排序
            var weightedDomains = from pair in apiDomainConnDict orderby pair.Value descending select pair;
            if (weightedDomains.Count() > 0)
            {
                return weightedDomains.First().Key;
            }
            return NetworkConfigLoader.Instance.GetServerDomain();
        }
    }

}