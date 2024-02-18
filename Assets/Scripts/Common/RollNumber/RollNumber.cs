using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace DanMuGame
{ 
    public class RollNumber : MonoBehaviour
    {
        private string format;
        private int value;
        private int showValue;
        private TextMeshProUGUI txt;
        public float time;

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
        public RollNumber SetValue(int value)
        {
            this.value = value;
            this.showValue = value;
            if (this.txt != null)
            {
                this.txt.text = string.Format(this.format, this.showValue);
            }
            return this;
        }

        /// <summary>
        /// 总是以当前的差值的time速率进行增加
        /// </summary>
        /// <param name="v"></param>
        public void SetTargetValue(int v)
        {
            this.value = v;
        }

        void UpdateJackpotTxt()
        {
            if (this.showValue == this.value) return;
            var v = Mathf.Lerp(this.showValue, this.value, (1 / this.time) * Time.deltaTime);
            var d = Mathf.Max(Mathf.Abs(v - this.showValue), 1);
            if (this.showValue > this.value)
            {
                //???????
                this.showValue = (int)(this.showValue - d);
                if (this.showValue < this.value) this.showValue = this.value;
            }
            else
            {
                //???????
                this.showValue = (int)(this.showValue + d);
                if (this.showValue > this.value) this.showValue = this.value;
            }

            this.txt.text = string.Format(this.format, this.showValue);
        }
    }
}