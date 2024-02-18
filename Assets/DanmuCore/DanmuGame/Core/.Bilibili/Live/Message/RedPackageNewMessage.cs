using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BilibiliUtilities.Live.Message
{
    [System.Serializable]
    public class RedPackageNewMessage : BaseMessage
    {
        /// <summary>
        /// 用户UID
        /// </summary>
        public long UserId;

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Username;

        /// <summary>
        /// 不知道是啥ID
        /// </summary>
        public long LotId;

        /// <summary>
        /// 前面还有几个红包在排队
        /// </summary>
        public int WaitCount;

        /// <summary>
        /// Action
        /// </summary>
        public string Action;

        /// <summary>
        /// 礼物名称
        /// </summary>
        public string GiftName;

        /// <summary>
        /// 礼物ID
        /// </summary>
        public long GiftId;

        /// <summary>
        /// 价格
        /// </summary>
        public int Price;


        public RedPackageNewMessage()
        {
        }

        public static RedPackageNewMessage JsonToRedPackageNewMessage(JObject json)
        {
            if (!"POPULARITY_RED_POCKET_NEW".Equals(json["cmd"].ToString()))
            {
                throw new ArgumentException("'cmd' 的值不是 'POPULARITY_RED_POCKET_NEW'");
            }

            var data = json["data"];
            try
            {
                var message = new RedPackageNewMessage();
                message.UserId = long.Parse(data["uid"].ToString());
                message.Username = data["uname"].ToString();
                message.LotId = long.Parse(data["lot_id"].ToString());
                message.WaitCount = int.Parse(data["wait_num"].ToString());
                message.Action = data["action"].ToString();
                message.GiftName = data["gift_name"].ToString();
                message.GiftId = long.Parse(data["gift_id"].ToString());
                message.Price = int.Parse(data["price"].ToString());
                message.Metadata = JsonConvert.SerializeObject(json);
                return message;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public static RedPackageNewMessage JsonToRedPackageNewMessage(string jsonStr)
        {
            try
            {
                var json = JObject.Parse(jsonStr);
                return JsonToRedPackageNewMessage(json);
            }
            catch (JsonReaderException)
            {
                throw new AggregateException("JSON字符串没有成功转换成Json对象");
            }
        }
    }
}