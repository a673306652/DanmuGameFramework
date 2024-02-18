using System.Collections;
using System.Collections.Generic;
using Daan;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DanMuGame
{
    public class NoticeManager : Manager<NoticeManager>
    {
        [SerializeField] private Transform noticeBannerParent;

        public Notice_Banner NoticeBanner(NoticeMsg msg, UnityAction onComplete = null)
        {
            var obj = ResourceManager.Instance.Spawn<Notice_Banner>($"Notice/横幅通知", noticeBannerParent);
            obj.SetUser(msg.id);
            obj.SetContent(string.Format(msg.formatter, msg.name, msg.count));
            obj.SetOnComplete(onComplete);
            return obj;
        }

        public Notice_Banner NoticeBanner(string id, string content)
        {
            var obj = ResourceManager.Instance.SpawnUI<Notice_Banner>($"Notice/横幅通知", noticeBannerParent);
            obj.SetUser(id);
            obj.SetContent(content);
            return obj;
        }

        public Notice_Banner NoticeBannerLong(string id, string content)
        {
            var obj = ResourceManager.Instance.SpawnUI<Notice_Banner>($"Notice/横幅通知1", noticeBannerParent);
            obj.SetUser(id);
            obj.SetContent(content);
            return obj;
        }

        // public Notice_Join NoticeJoin(string id, string path)
        // {
        //     var user = UserManager.Instance.GetUser(id);
        //     if (user != null)
        //     {
        //         var obj = ResourceManager.Instance.SpawnUI<Notice_Join>($"{path}", noticeBannerParent);
        //         obj.SetUser(id);
        //         return obj;
        //     }
        //     return null;
        // }

        // public Notice_Score NoticeScore(string id, int score, int level = 0)
        // {
        //     var user = UserManager.Instance.GetUser(id);
        //     if (user != null)
        //     {
        //         var obj = ResourceManager.Instance.SpawnUI<Notice_Score>($"Notice/积分通知{level}");
        //         obj.SetUser(id);
        //         obj.SetScore(score);
        //         return obj;
        //     }
        //     return null;
        // }

        /// <summary>
        /// 非预设的一些自定义需求，记住这里是传完整路径哈
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public Notice_Custom NoticeCustom(string id, string path = "", object[] p = null)
        {
            var user = UserManager.Instance.GetUser(id);
            if (user != null)
            {
                var obj = ResourceManager.Instance.SpawnUI<Notice_Custom>($"{path}");
                obj.SetUser(id);
                obj.SetParams(p);
                return obj;
            }
            return null;
        }

        public struct NoticeMsg
        {
            public string id;
            public string formatter;
            public int count;
            public string name;
        }
    }
}
