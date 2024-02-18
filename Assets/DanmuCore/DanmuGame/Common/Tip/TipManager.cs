using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;

namespace DanMuGame
{
    public class TipManager : Manager<TipManager>
    {
        public Transform tipsParent;


        public List<TipUIItem> topList = new List<TipUIItem>();
        public List<TipUIItem> list = new List<TipUIItem>();

        public void ShowTip(string content, string type = "", string id = "")
        {
            //@Daan ������return�˹�
            return;
            //�����Type��Ҫ�ѵ�
            if (!string.IsNullOrEmpty(type))
            {
                for (int i = 0; i < this.list.Count; i++)
                {
                    var item = this.list[i];
                    if (string.IsNullOrEmpty(item.type)) continue;

                    if (type == item.type && id == item.id)
                    {
                        item.count++;
                        if (!this.topList.Contains(item))
                        {
                            this.topList.Add(item);
                        }
                        item.Show($"{content} x{item.count + 1}", type, id);
                        item.autoDespawnTimer = item.autoDespawnTime;
                        return;
                    }
                }
            }
            var tip = ResourceManager.Instance.Spawn<TipUIItem>("Common/Tip", this.tipsParent);
            tip.Show(content, type, id);
            this.list.Add(tip);
            tip.transform.SetSiblingIndex(this.topList.Count);
        }


        public void RemoveTip(TipUIItem tip)
        {
            this.list.Remove(tip);
            this.topList.Remove(tip);
        }
    }
}