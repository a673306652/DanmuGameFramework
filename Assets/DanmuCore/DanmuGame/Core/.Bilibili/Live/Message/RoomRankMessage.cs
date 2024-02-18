using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

namespace BilibiliUtilities.Live.Message
{
    public class RoomRankMessage : BaseMessage
    {
        public class Data
        {
            public long UserId;
            public string UserName;
            public string FaceUrl;
            public int Score;
            public int Rank;
            public int GuardLevel;
        }
        public List<Data> UserList = new List<Data>();
        public string RankType;

        public static RoomRankMessage JsonToRoomRankMessage(JObject json)
        {
            var result = new RoomRankMessage
            {
                RankType = json["data"]["rank_type"].ToString(),
                Metadata = JsonConvert.SerializeObject(json)
            };
            var array = JArray.Parse(json["data"]["list"].ToString());
            for (int i = 0; i < array.Count; i++)
            {
                result.UserList.Add(new Data
                {
                    UserId = long.Parse(array[i]["uid"].ToString()),
                    UserName = array[i]["uname"].ToString(),
                    FaceUrl = array[i]["face"].ToString(),
                    Score = int.Parse(array[i]["score"].ToString()),
                    Rank = int.Parse(array[i]["rank"].ToString()),
                    GuardLevel = int.Parse(array[i]["guard_level"].ToString()),
                });
            }
            return result;
        }

        public static RoomRankMessage JsonToRoomRankMessage(string jsonStr)
        {
            try
            {
                return JsonToRoomRankMessage(JObject.Parse(jsonStr));
            }
            catch (JsonReaderException)
            {
                throw new AggregateException("JSON字符串没有成功转换成Json对象");
            }
        }
    }
}