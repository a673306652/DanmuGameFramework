using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Daan;

namespace DanMuGame
{
    public class CarouselUIPanel : PoolObject
    {

        [HideInInspector] public CarouselUISystem system;
        [HideInInspector] public RectTransform rect;
        public float duration;
        public float CD;
        private float lastActiveTime = -1000;
        private float timer;
        public bool once;
        public bool insert;

        private void Awake()
        {
            this.rect = this.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            this.timer = this.duration;
            this.lastActiveTime = Time.realtimeSinceStartup;
        }

        public bool CheckCD()
        {
            return Time.realtimeSinceStartup - this.lastActiveTime >= this.CD;
        }

        private void Update()
        {
            if (this.timer > 0)
            {
                this.timer -= Time.deltaTime;
            }
            if (this.timer <= 0)
            {
                if (this.system.MoveNext())
                {
                    this.timer = float.MaxValue;
                }
            }
        }
    }
}
