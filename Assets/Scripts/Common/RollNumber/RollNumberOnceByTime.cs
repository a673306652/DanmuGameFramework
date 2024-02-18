using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace DanMuGame
{
   
    public class RollNumberOnceByTime : MonoBehaviour
    {
        private string format;
        private int value;
        private int startValue;
        private int showValue;
        private TextMeshProUGUI txt;
        public float time;
        private float timer;

        private void Awake()
        {
            this.txt = this.GetComponent<TextMeshProUGUI>();
            this.format = this.txt.text;
            this.SetValue(0);
        }

        private void Update()
        {
            this.UpdateJackpotTxt();
        }

        /// <summary>
        /// ?????
        /// </summary>
        /// <param name="value"></param>
        public RollNumberOnceByTime SetValue(int value)
        {
            this.value = value;
            this.showValue = value;
            if (this.txt != null)
            {
                this.txt.text = string.Format(this.format, this.showValue);
            }
            this.timer = 0;
            return this;
        }

        /// <summary>
        /// 总是以当前的差值的time速率进行增加
        /// </summary>
        /// <param name="v"></param>
        public void SetTargetValue(int v, int start = 0)
        {
            this.value = v;
            this.startValue = start;
            this.timer = 0;
        }

        void UpdateJackpotTxt()
        {
            if (this.showValue == this.value) return;
            this.timer += Time.deltaTime;
            this.showValue = (int)Mathf.Lerp(this.startValue, this.value, this.timer / this.time);
            this.txt.text = string.Format(this.format, this.showValue);
        }
    }
}