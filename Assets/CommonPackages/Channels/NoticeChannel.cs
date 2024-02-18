namespace Channels.NoticeChannel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class NoticeChannel
    {
        // 排序队列，存储按优先级排序的签名映射信息<placement,<signature,priority>[]>
        private Dictionary<string, List<(string, int)>> sortedListData = new Dictionary<string, List<(string, int)>>();
        // 签名队列，相同签名的消息可以被合并
        private Dictionary<string, List<string>> signatureData = new Dictionary<string, List<string>>();
        // 签名配置，按Placement设置
        private Dictionary<string, NoticeChannelPlacement> placements = new Dictionary<string, NoticeChannelPlacement>();
        // 消息体字典
        private Dictionary<string, NoticeChannelMsg> msgData = new Dictionary<string, NoticeChannelMsg>();
        // 运行中的消息，按Placement设置
        private Dictionary<string, string> runningMsgs = new Dictionary<string, string>();

        private NoticeChannel()
        {

        }

        public static NoticeChannel New()
        {
            return new NoticeChannel();
        }

        public NoticeChannel Configure(string placement, NoticeChannelPlacement config)
        {
            placements[placement] = config;
            return this;
        }

        public NoticeChannel Configure(string placement, int maxCount = -1)
        {
            NoticeChannelPlacement config;
            if (!placements.ContainsKey(placement))
            {
                config = new NoticeChannelPlacement();
            }
            config.MaxCount = maxCount;
            placements[placement] = config;
            return this;
        }

        public void Put(string userId,
                        string msgType,
                        string placement,
                        int priority,
                        float duration,
                        object payload,
                        NoticeChannelSpawner onSpawn,
                        NoticeChannelCombiner onCombine,
                        NoticeChannelDisposer onDispose,
                        bool autoSort = true)
        {
            Put(new NoticeChannelMsg()
            {
                UserId = userId,
                MsgType = msgType,
                Placement = placement,
                Priority = priority,
                DisplayDuration = duration,
                Payload = payload,
                OnSpawn = onSpawn,
                OnCombine = onCombine,
                OnDispose = onDispose,
            }, autoSort);
        }

        public void Put(NoticeChannelMsg msg, bool autoSort = true)
        {
            msg.MsgId = Guid.NewGuid().ToString("N");
            msg.Channel = this;
            msgData[msg.MsgId] = msg;

            if (!signatureData.ContainsKey(msg.Signature))
            {
                signatureData[msg.Signature] = new List<string>();
            }

            var sL = signatureData[msg.Signature];
            sL.Add(msg.MsgId);

            if (msg.IsCombinable && sL.Count > 1)
            {
                // 触发Combine
                var baseMsg = msgData[sL[0]];
                var remains = sL.GetRange(1, sL.Count - 1);
                for (var i = 0; i < remains.Count; i++)
                {
                    // 运行中的消息不可被Combine
                    if (IsRunning(baseMsg.Placement, remains[i])) continue;

                    baseMsg = baseMsg.OnCombine(msgData[remains[i]], baseMsg);
                    msgData.Remove(remains[i]);
                    sL.Remove(remains[i]);
                }
                msgData[sL[0]] = baseMsg;
            }
            // 无配置的Placement不做任何处理
            if (placements.ContainsKey(msg.Placement))
            {
                var placementConfig = placements[msg.Placement];

                if (placementConfig.MaxCount >= 0 && sL.Count > placementConfig.MaxCount)
                {
                    var toRemove = sL[sL.Count - 1];
                    var toRemoveIns = msgData[toRemove];
                    sL.RemoveAt(sL.Count - 1);
                    msgData.Remove(toRemove);
                    toRemoveIns.OnDispose?.Invoke(toRemoveIns);
                }
            }

            signatureData[msg.Signature] = sL;

            if (!sortedListData.ContainsKey(msg.Placement))
            {
                sortedListData[msg.Placement] = new List<(string, int)>();
            }

            var pL = sortedListData[msg.Placement];
            pL.Add((msg.Signature, msg.Priority));
            if (autoSort)
            {
                // 由小至大排序
                pL.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            }
            sortedListData[msg.Placement] = pL;
        }

        public void Sort(string placement)
        {
            var pL = sortedListData[placement];
            // 由小至大排序
            pL.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            sortedListData[placement] = pL;
        }

        public void SortAll()
        {
            var allPLs = placements.Keys.ToList();
            allPLs.ForEach(k =>
            {
                Sort(k);
            });
        }

        private bool IsRunning(string placement, string msgId)
        {
            if (!runningMsgs.ContainsKey(placement)) return false;
            return runningMsgs[placement] == msgId;
        }

        /// <summary>
        /// 此方法会同时修改sortedList/signatureData/msgData
        /// </summary>
        private void RemoveMsg(string msgId)
        {
            if (!msgData.ContainsKey(msgId)) return;
            var baseMsg = msgData[msgId];
            var sL = signatureData[baseMsg.Signature];
            sL.Remove(msgId);

            // 若签名队列为空，同时将其签名从排序队列移除
            if (sL.Count < 1)
            {
                var pL = sortedListData[baseMsg.Placement];
                pL.Remove((baseMsg.Signature, baseMsg.Priority));
                sortedListData[baseMsg.Placement] = pL;
            }

            signatureData[baseMsg.Signature] = sL;
            msgData.Remove(msgId);
            baseMsg.OnDispose?.Invoke(baseMsg);
        }

        public void Tick(float deltaTime)
        {
            var placements = this.placements.Keys;

            foreach (var p in placements)
            {
                if (!runningMsgs.ContainsKey(p))
                {
                    if (!sortedListData.ContainsKey(p))
                    {
                        sortedListData[p] = new List<(string, int)>();
                    }
                    var pL = sortedListData[p];
                    if (pL.Count < 1)
                        continue;

                    var batch = pL[0];
                    pL.RemoveAt(0);
                    sortedListData[p] = pL;

                    var sL = signatureData[batch.Item1];
                    if (sL.Count < 1)
                        continue;
                    var batchMsg = sL[0];
                    sL.RemoveAt(0);
                    signatureData[batch.Item1] = sL;

                    runningMsgs[p] = batchMsg;
                    msgData[batchMsg].OnSpawn?.Invoke(msgData[batchMsg]);
                }

                if (AddTick(deltaTime, runningMsgs[p]))
                {
                    RemoveMsg(runningMsgs[p]);
                    runningMsgs.Remove(p);
                }
            }
        }

        private bool AddTick(float deltaTime, string msgId)
        {
            var msg = msgData[msgId];
            msg.DeltaTime += deltaTime;
            msgData[msgId] = msg;
            return msg.DeltaTime >= msg.DisplayDuration;
        }
    }

    public delegate void NoticeChannelSpawner(NoticeChannelMsg self);
    public delegate NoticeChannelMsg NoticeChannelCombiner(NoticeChannelMsg other, NoticeChannelMsg self);
    public delegate void NoticeChannelDisposer(NoticeChannelMsg self);

    public struct NoticeChannelPlacement
    {
        public int MaxCount; // 最大容量，超过此容量将开始弃置消息，取负值时作无限处理
    }

    public struct NoticeChannelMsg
    {
        public NoticeChannel Channel;
        public string MsgId; // 消息Id，唯一
        public int Priority; // 排序等级
        public string Signature => $"{MsgType}~{UserId}"; // 消息签名
        public string Placement; // 消息外显位置，同位置消息采用一套配置
        public string UserId; // 来自哪个用户，当属于系统消息时，用户Id需要自定义
        public string MsgType; // 同类消息可以被合并
        public float DisplayDuration; // 消息展示时长
        public bool IsSystemMsg; // 是否为系统消息
        public bool IsCombinable => null != OnCombine; // 是否可被合并

        public NoticeChannelSpawner OnSpawn; // 消息生成
        public NoticeChannelCombiner OnCombine; // 消息合并
        public NoticeChannelDisposer OnDispose; // 消息弃置/回收

        public float DeltaTime;
        public object Payload; // 携带数据，此数据将在创建消息外显实体时回传
    }
}