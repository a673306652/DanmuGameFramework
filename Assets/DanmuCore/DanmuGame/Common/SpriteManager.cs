using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
using System;
using UnityEditor;
using System.IO;


namespace DanMuGame
{
    public class SpriteManager : Manager<SpriteManager>
    {
        private RandomList<Sprite> emojiSprites = new RandomList<Sprite>();

        private void Awake()
        {
            StreamingAssetsLoader.Instance.LoadTextureByPath("Emoji", (list) =>
             {
                if (null == list) return;
                 foreach (var item in list)
                 {
                     this.emojiSprites.Add(Sprite.Create(item, new Rect(0, 0, item.width, item.height), Vector2.one * 0.5F));
                 }
             });
        }

        public Sprite GetEmoji(int index = -1)
        {
            if (index < 0)
            {
                return this.emojiSprites.GetRandom(false);
            }
            else
            {
                if (index >= this.emojiSprites.Count) index = this.emojiSprites.Count - 1;
                return this.emojiSprites[index];
            }
        }
    }
}
