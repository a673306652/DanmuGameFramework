using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Daan;
using System;

namespace DanMuGame
{
    public class VotePanel : PoolObject
    {
        public int reward;

        public GameObject prefab;
        public Text tittleTxt;
        public Text rewardTxt;
        public Text noticeTxt;
        public Transform optionParent;
        private List<Text> optionsTxt = new List<Text>();
        private VoteManager.Data data;
        private string originNotice;


        private void Awake()
        {
            this.prefab.gameObject.SetActive(false);
            this.originNotice = this.noticeTxt.text;
            this.rewardTxt.text = string.Format(this.rewardTxt.text, this.reward);
            this.CreateVote();
        }

        private void OnEnable()
        {
            if (this.data != null)
            {
                this.data.active = true;
            }
            this.noticeTxt.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (this.data != null)
            {
                //this.data.active = false;
            }
        }

        public void CreateVote()
        {
            var t = this.optionParent.GetComponentsInChildren<Text>();
            this.CreateVote(this.tittleTxt.text, Array.ConvertAll(t, s => s.text));
        }

        public void CreateVote(string tittle, params string[] options)
        {
            this.tittleTxt.text = tittle;
            this.optionParent.DestroyAllChildren();
            this.data = VoteManager.Instance.CreateVote(this.OnVote, options);
            for (int i = 0; i < this.optionsTxt.Count; i++)
            {
                GameObject.Destroy(this.optionsTxt[i].gameObject);
                i--;
            }
            this.optionsTxt.Clear();
            for (int i = 0; i < options.Length; i++)
            {
                var obj = GameObject.Instantiate(this.prefab, this.optionParent);
                obj.gameObject.SetActive(true);
                var text = obj.GetComponentInChildren<Text>();
                text.text = options[i];
                this.optionsTxt.Add(text);
            }
        }

        public void OnVote(VoteManager.Data data, string id, string option)
        {
            var user = UserManager.Instance.GetUser(id);
            if (user != null)
            {
                user.Set<int>("points", points => points + reward);
                user.SaveData();
                this.noticeTxt.text = string.Format(this.originNotice, user.nickname, this.reward);
                this.noticeTxt.gameObject.SetActive(true);
            }
        }
    }
}