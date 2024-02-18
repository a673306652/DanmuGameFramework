using System.Collections;
using System.Collections.Generic;
using Daan;
using UnityEngine;
using UnityEngine.UI;

namespace DanMuGame
{
    public class UserImage : PoolObject
    {

        [ReadOnlyInInspector] public string id;
        [ReadOnlyInInspector] public string url;
        [ReadOnlyInInspector] public bool loaded = false;
        public Image icon;
        private Sprite originSprite;
        private string faceUrl;

        private void Awake()
        {
            this.Init();
        }

        public override void Init()
        {
            base.Init();
            if (null == this.icon)
                this.icon = this.transform.Find<Image>("Mask/Image");
            this.originSprite = this.icon.sprite;
        }

        public void SetUser(string id)
        {
            if (this.id != id)
            {
                this.icon.sprite = this.originSprite;
                this.id = id;
                this.loaded = false;
            }
        }

        public void SetUrl(string url, string faceUrl)
        {
            if (this.url != url)
            {
                this.id = "";
                this.icon.sprite = this.originSprite;
                this.url = url;
                this.faceUrl = faceUrl;
                var sprite = DynamicAtlasManager.Instance.GetUserIcon(this.faceUrl);
                if (null == sprite)
                {
                    var self = this;
                    DynamicAtlasManager.Instance.DownloadUserIcon(this.faceUrl, (s) =>
                    {
                        self.icon.sprite = s;
                    });
                }
                else
                {
                    this.icon.sprite = sprite;
                }
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
                // id模式
                var user = UserManager.Instance.GetUser(this.id);
                if (user != null && user.icon != null)
                {
                    this.loaded = true;
                    this.icon.sprite = user.icon;
                }
            }
        }
    }
}
