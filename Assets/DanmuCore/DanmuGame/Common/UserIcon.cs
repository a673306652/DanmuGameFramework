using System.Collections;
using System.Collections.Generic;
using Daan;
using UnityEngine;

namespace DanMuGame
{
    public class UserIcon : PoolObject
    {
        [ReadOnlyInInspector] public string id;
        [ReadOnlyInInspector] public SpriteRenderer icon;
        [ReadOnlyInInspector] public SpriteRenderer core;
        [ReadOnlyInInspector] public bool loaded = false;

        private Sprite originSprite;
        private Vector3 originScale;
        private SpriteRenderer bgSprite;

        private void Awake()
        {
            this.Init();
        }

        public override void Init()
        {
            base.Init();
            this.icon = this.transform.Find<SpriteRenderer>("Icon");
            this.core = this.transform.Find<SpriteRenderer>("Core");
            this.originSprite = this.icon.sprite;
            this.originScale = this.icon.transform.localScale;
            this.bgSprite = this.transform.Find<SpriteRenderer>("BG");
        }

        public void SetUser(string id)
        {
            if (this.id != id)
            {
                var mat = DynamicAtlasManager.Instance.GetMaterial(id);
                this.icon.transform.localScale = this.originScale;
                this.id = id;
                // 没有缓存材质球的时候才需要重刷sprite
                if (null == mat)
                    this.icon.sprite = this.originSprite;

                // 这里若this.id为空，可能导致多个用户头像互相闪，先跳过了
                if (null != this.core && this.id != null)
                {
                    mat = DynamicAtlasManager.Instance.GetMaterial(this.id);
                    if (null == mat)
                    {
                        mat = this.core.material;
                        DynamicAtlasManager.Instance.SetMaterial(this.id, mat);
                    }
                    mat.SetTexture("Icon", this.icon.sprite.texture);
                    this.core.sharedMaterial = mat;
                }
                this.loaded = false;
            }
        }

        public void SetBgColor(Color color)
        {
            this.bgSprite.color = color;
        }

        private void Update()
        {
            if (!this.loaded)
            {
                var user = UserManager.Instance.GetUser(this.id);
                if (user != null && user.icon != null)
                {
                    this.loaded = true;
                    var width = user.icon.texture.width;
                    var height = user.icon.texture.height;
                    var scale = Mathf.Max(width, height) / 100F;
                    // var scale = Mathf.Max(width, height) / 3000F;

                    this.icon.sprite = user.icon;
                    if (null != this.core)
                    {
                        var mat = DynamicAtlasManager.Instance.GetMaterial(this.id);
                        if (null == mat)
                        {
                            mat = this.core.material;
                            DynamicAtlasManager.Instance.SetMaterial(this.id, mat);
                        }
                        mat.SetTexture("Icon", this.icon.sprite.texture);
                        this.core.sharedMaterial = mat;
                    }
                    this.icon.transform.localScale = Vector3.one / scale;
                }
            }
        }
    }
}
