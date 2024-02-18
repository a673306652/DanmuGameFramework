using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
using UnityEngine.UI;

namespace DanMuGame
{
    public class BackgroundText : AutoDespawnObject
    {
        private Text txt;
        private Canvas canvas;

        public override void Init()
        {
            base.Init();
            this.canvas = this.GetComponentInChildren<Canvas>();
            this.txt = this.GetComponentInChildren<Text>();
        }

        protected override void Update()
        {
            base.Update();
        }

        public void SetOrder(int order)
        {
            this.canvas.sortingOrder = order;
        }

        public void SetContent(string content)
        {
            this.txt.text = content;
        }
    }
}
