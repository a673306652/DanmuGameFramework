using System.Net.Mime;
namespace Services.Feishu
{
    using System.Collections.Generic;
    using Modules.Network;
    using Modules.Utilities;
    using UnityEngine;

    public class FeedbackOperation : RequestOperation
    {
        public FeedbackOperation() { }

        public override string Route
        {
            get
            {
                return "https://open.feishu.cn/open-apis/bot/v2/hook/6f97ed5d-4396-4119-aa1f-596386e40991";
            }
        }

        public override Method Method
        {
            get
            {
                return Method.Post;
            }
        }
    }
    
    public class FeedbackOperation2 : RequestOperation
    {
        public FeedbackOperation2() { }

        public override string Route
        {
            get
            {
                return "https://open.feishu.cn/open-apis/bot/v2/hook/f61c56b7-6bef-422d-8441-60a4d9c5d837";
            }
        }

        public override Method Method
        {
            get
            {
                return Method.Post;
            }
        }
    }

    public class FeedbackRequestOutput : RequestOutput
    {

    }

    public class FeedbackRequestInput : RequestInput
    {
        public string msg_type = "text";
        public FeedbackContentBody content;
    }

    public class FeedbackContentBody
    {
        public string text;
    }

    public class FeedbackPayload
    {
        // public string uid = CommonDataCollection.UserId; //用户ID
        // public string app_pkg = CommonDataCollection.AppPkg; //应用包名
        // public string app_id = CommonDataCollection.AppId; //应用内部代号
        public string app_version = Application.version; //应用版本
        public string platform = "Windows"; //平台 iOS或 Android
        // public string country = CommonDataCollection.Country; //国家
        public string timestamp_milli = TimeUtils.GetCurrentUTCMilliSeconds().ToString(); //毫秒级时间戳
        public string title;
        public string message;

        public string Format()
        {
            return $"应用版本：{app_version}\n平台：{platform}\n标题：{title}\n消息内容：{message}";
        }
    }
}