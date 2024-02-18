using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Daan
{
    public partial class Message
    {
        public class CommonCmd
        {
            public static string GameStart = "GameStart";
            public static string GameOver = "GameOver";
        }

        public class Msg
        {
            /// <summary>
            /// Id
            /// </summary>
            private readonly string id;
            /// <summary>
            /// 内容
            /// </summary>
            private readonly object content;

            public Msg(string id, object content)
            {
                this.id = id;
                this.content = content;
            }
            public string GetId()
            {
                return this.id;
            }
            public object GetContent()
            {
                return this.content;
            }
        }

        private string channel;
        private Queue<Msg> messageQueue = new Queue<Msg>();
        private Dictionary<string, Action<object>> dict = new Dictionary<string, Action<object>>();
        private bool isClear = false;

        public Message(string channel)
        {

            Message self = this as Message;
            this.channel = channel;
        }
        public string GetChannel()
        {
            return this.channel;
        }
        public override string ToString()
        {
            return this.GetChannel();
        }
        public void Update()
        {
            try
            {

                Queue<Msg> tempQueue = null;
                lock (this.messageQueue)
                {
                    tempQueue = new Queue<Msg>(this.messageQueue);
                    this.messageQueue.Clear();
                }
                Msg msg = null;
                while (tempQueue.Count > 0)
                {
                    if (this.isClear) break;
                    msg = tempQueue.Dequeue();

                    ReadMessage(msg.GetId(), msg.GetContent());
                }

            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        public void Clear()
        {
            lock (this.messageQueue)
            {
                dict.Clear();
                messageQueue.Clear();
                this.isClear = true;
            }
        }


        public void Broadcast(string str, object content = null, bool rightNow = true)
        {
            lock (handlers)
            {
                if (rightNow)
                {
                    this.ReadMessage(str, content);
                }
                else
                {
                    this.SendMessage(str, content);
                }
            }
        }

        public void ReadMessage(string str, object content)
        {
            Action<object> handlers = null;
            if (this.dict.TryGetValue(str, out handlers))
            {
                if (handlers != null)
                {
                    handlers(content);
                    return;
                }
            }
        }

        public void AddListener(string id, Action<object> handler)
        {

            if (handler == null)
            {
                Debug.LogError("妈卖批，监听了个卵");
                return;
            }
            Action<object> h = null;
            //如果不存在当前消息类型，则添加
            if (!this.dict.TryGetValue(id, out h) || h == null)
            {
                this.dict[id] = handler;
            }
            else
            {
                h += handler;
                this.dict[id] = h;
            }
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        public void RemoveListener(string id, Action<object> handler)
        {
            if (handler == null)
            {
                Debug.LogError("妈卖批，监听了个卵");
                return;
            }
            if (this.dict.ContainsKey(id))
            {
                this.dict[id] -= handler;
            }
        }

        /// <summary>
        /// 发送消息到消息队列中
        /// </summary>
        public void SendMessage(string id, object content)
        {
            Msg msg = new Msg(id, content);

            lock (this.messageQueue)
            {
                this.isClear = false;
                this.messageQueue.Enqueue(msg);
            }
        }


        /// <summary>
        /// 立即发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="content"></param>
        public void SendMessageRightNow(string id, object type)
        {
            Action<object> handlers = null;
            if (this.dict.TryGetValue(id, out handlers))
            {
                if (handlers != null)
                {
                    handlers(type);
                    return;
                }
            }
        }

        private static Dictionary<string, Message> handlers = new Dictionary<string, Message>();
        private static List<Message> list = new List<Message>();
        public static Message Main = Message.Get("Main");



        public static Message Get(string channel)
        {
            if (!handlers.ContainsKey(channel))
            {
                handlers.Add(channel, new Message(channel));
                list.Add(handlers[channel]);
            }
            return handlers[channel];
        }

        public static void UpdateAll()
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Update();
            }
        }
        public static void SendMessage(string str, object content = null, bool rightNow = true)
        {
            lock (handlers)
            {
                foreach (Message item in handlers.Values)
                {
                    if (rightNow)
                    {
                        item.ReadMessage(str, content);
                    }
                    else
                    {
                        item.SendMessage(str, content);
                    }
                }
            }
        }

        public static void ClearAll()
        {
            foreach (Message item in handlers.Values)
            {
                item.Clear();
            }
        }
    }
}