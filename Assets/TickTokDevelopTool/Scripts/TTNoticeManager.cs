using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static NoticeQueue;


public static class TTNoticeManager
{
    private static Dictionary<string, NoticeQueue> processDict;
    private static int TaskID;


    public static string[] AllTag()
    {
        return processDict.Keys.ToArray();
    }

    public static string GetFreePass()
    {
        var c = processDict.ToList();

        bool hasFreePass = false;
        string passName = string.Empty;
        for (var i = 0; i < c.Count; i++)
        {
            if (c[i].Value.noticeQueue.Count <= 0)
            {
                hasFreePass = true;
                passName = c[i].Key;
                break;
            }
        }

        if (hasFreePass)
        {
            return passName;
        }
        else
        {
            c.Sort((a, b) => { return b.Value.noticeQueue.Count.CompareTo(a.Value.noticeQueue.Count); });
            return c[0].Key;
        }
    }

    public static void InitNoticeList(string[] Attributes)
    {
        TaskID = 0;
        processDict = new Dictionary<string, NoticeQueue>();
        for (int i = 0; i < Attributes.Length; i++)
        {
            processDict.Add((Attributes[i]), new NoticeQueue());
        }
    }

    public static void UpdateNoticeSystem()
    {
        var list = processDict.ToList();
        list.Sort((a, b) =>
        {
            int p1 = 0;
            FindHighstNotice(a.Value, out p1);
            int p2 = 0;
            FindHighstNotice(a.Value, out p2);
            return p2.CompareTo(p1);
        });

        for (var i = 0; i < list.Count; i++)
        {
            var value = list[i];

            if (value.Value.isPlaying)
            {
                value.Value.safeTime += Time.deltaTime;

                if (value.Value.safeTime > 8f)
                {
                    value.Value.safeTime = 0;
                    value.Value.isPlaying = false;
                }
            }
            else
            {
                value.Value.safeTime = 0;
            }

            if (!value.Value.isPlaying && value.Value.noticeQueue.Count > 0)
            {
                int index = 0;
                var noticeTask = FindHighstNotice(value.Value, out index);

                processDict[value.Key].noticeQueue.RemoveAt(index);

                for (var i1 = 0; i1 < noticeTask.Keys.Length; i1++)
                {
                    processDict[noticeTask.Keys[i1]].TaskID = noticeTask.TaskID;
                    processDict[noticeTask.Keys[i1]].playingPriority = noticeTask.Priority;
                    processDict[noticeTask.Keys[i1]].isPlaying = true;
                }

                noticeTask.Run();
            }
        }
    }

    public static NoticeItem EnqueueFreeNotice(
        int priority,
        UnityAction<NoticeItem> NoticeFunction,
        UnityAction OnComplete, string id,
        float duration = -1f)
    {
        NoticeItem item = new NoticeItem();
        item.Id = id;
        var currentId = TaskID;

        var key = GetFreePass();

        processDict[key].noticeQueue.Add(item.SetNoticeTask(new string[]{key}, () =>
        {
        
                processDict[key].safeTime = 0f;
    

            NoticeFunction?.Invoke(item);
        }, () =>
        {
            for (var i1 = 0; i1 < item.Keys.Length; i1++)
            {
                if (processDict[key].TaskID == item.TaskID)
                {
                    processDict[key].isPlaying = false;
                }
            }

            OnComplete?.Invoke();
        }, priority, currentId, duration));
        TaskID += 1;

        return item;
    }

    public static NoticeItem EnqueueNotice(
        bool jumperMode,
        string[] key,
        int priority,
        UnityAction<NoticeItem> NoticeFunction,
        UnityAction OnComplete, string id,
        float duration = -1f)
    {
        NoticeItem item = new NoticeItem();
        item.Id = id;
        if (processDict.ContainsKey(key[0]))
        {
            if (jumperMode)
            {
                if (processDict[key[0]].noticeQueue.Count > 0)
                {
                    int index = 0;
                    var noticeTask = FindHighstNotice(processDict[key[0]], out index);
                    if (noticeTask.Priority < 100)
                    {
                        processDict[key[0]].isPlaying = false;
                    }
                }
                else
                {
                    if (processDict[key[0]].playingPriority < 1000)
                    {
                        processDict[key[0]].isPlaying = false;
                    }
                }
            }

            var currentId = TaskID;

            // item.gameObject.SetActive(false);
            processDict[key[0]].noticeQueue.Add(item.SetNoticeTask(key, () =>
            {
                for (var i = 0; i < key.Length; i++)
                {
                    processDict[key[i]].safeTime = 0f;
                }

                NoticeFunction?.Invoke(item);
            }, () =>
            {
                for (var i1 = 0; i1 < item.Keys.Length; i1++)
                {
                    if (processDict[key[i1]].TaskID == item.TaskID)
                    {
                        processDict[key[i1]].isPlaying = false;
                    }
                }

                OnComplete?.Invoke();
            }, priority, currentId, duration));
            TaskID += 1;
            // processDict[key[0]].noticeQueue.Sort((a, b) => { return b.Priority.CompareTo(a.Priority); });
        }
        else
        {
            Debug.LogError("Try Enqueue A Invalid key,Check The Key => " + key);
            Debug.LogError("传入的key找不到匹配的队列，请检查key是否正确 " + key);
        }

        return item;
    }

    public static void TryCombine(string name, string id, UnityAction<NoticeItem> onLoop,
        OnNoticeQueueCombine onCombine)
    {
        var q = processDict[name];
        var items = q.noticeQueue.Where(i => i.Id == id).ToArray();
        Debug.Log("尝试合并" + items.Length);
        if (items.Length > 1)
        {
            var baseItem = items[0];
            for (var i = 0; i < items.Length; i++)
            {
                onLoop(items[i]);
                q.noticeQueue.Remove(items[i]);
            }

            baseItem = onCombine(baseItem);
            q.noticeQueue.Add(baseItem);
            processDict[name] = q;
        }
    }

    private static NoticeItem FindHighstNotice(NoticeQueue queue, out int index)
    {
        if (queue.noticeQueue.Count <= 0)
        {
            index = 0;
            return null;
        }

        int indexx = 0;
        int priority = 0;

        for (var i = 0; i < queue.noticeQueue.Count; i++)
        {
            if (queue.noticeQueue[i].Priority > priority)
            {
                priority = queue.noticeQueue[i].Priority;
                indexx = i;
            }
        }

        index = indexx;
        return queue.noticeQueue[indexx];
    }
}