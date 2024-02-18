using System.Collections;
using System.Collections.Generic;
using Daan;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DanMuGame
{
    public class UserName_TMP : PoolObject
    {
        [ReadOnlyInInspector] public string id;
        [ReadOnlyInInspector] public string url;
        private TextMeshPro txt;

        public string text
        {
            get
            {
                return this.txt.text;
            }
            set
            {
                this.txt.text = value;
            }
        }
        public Color color
        {
            get
            {
                return this.txt.color;
            }
            set
            {
                this.txt.color = value;
            }
        }

        [ReadOnlyInInspector] public bool loaded = false;

        private void Awake()
        {
            this.Init();
        }

        public override void Init()
        {
            base.Init();
            this.txt = this.GetComponent<TextMeshPro>();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            this.loaded = false;
        }

        public void SetUser(string id)
        {
            if (this.id != id)
            {
                this.id = id;
                this.loaded = false;
            }
        }

        public void SetUrl(string url, string text)
        {
            if (this.url != url)
            {
                this.id = "";
                this.url = url;
                this.text = text;
                this.loaded = true;
            }
        }

        private void Update()
        {
            if (!this.loaded)
            {
                // url模式
                if (string.IsNullOrEmpty(this.id))
                {
                    return;
                }
                var user = UserManager.Instance.GetUser(this.id);
                if (user != null)
                {
                    this.text = user.nickname;
                    this.loaded = true;
                }
            }
        }
    }
}
