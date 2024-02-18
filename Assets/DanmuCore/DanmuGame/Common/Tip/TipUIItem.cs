using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Daan;

namespace DanMuGame
{
    public class TipUIItem : AutoDespawnObject
    {
        public string id;
        public string type;
        public int count;
        [HideInInspector]
        public Text text;
        private Animator anim;

        public override void Init()
        {
            base.Init();
            this.text = this.GetComponentInChildren<Text>();
            this.anim = this.GetComponentInChildren<Animator>();
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            this.count = 0;
            TipManager.Instance.RemoveTip(this);
        }

        public void Show(string content, string type, string id = null)
        {
            this.type = type;
            this.id = id;
            this.text.text = content;
            this.anim.Play("Show", 0, 0);
        }
    }

}