using System.Collections;
using System.Collections.Generic;
using Daan;
using DanMuGame;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DanMuGame
{
    public class Notice_User : AutoDespawnObject
    {
        public Text rankTxt;
        public TextMeshProUGUI tmp;
        private UserImage userIcon;
        private UserName userName;
        public override void Init()
        {
            base.Init();
            this.userIcon = this.GetComponentInChildren<UserImage>();
            this.userName = this.GetComponentInChildren<UserName>();
        }

        public void SetUser(string id)
        {
            this.userIcon.gameObject.SetActive(true);
            this.userName.gameObject.SetActive(true);
            this.userIcon.SetUser(id);
            this.userName.SetUser(id);
            if (string.IsNullOrEmpty(id))
            {
                this.userIcon.gameObject.SetActive(false);
                this.userName.gameObject.SetActive(false);
            }
        }

        public void SetRank(int worldRank)
        {
            if (worldRank < 100 && null != rankTxt)
            {
                rankTxt.text = $"世界第{NumberToChinese(worldRank + 1)}";
            }
            
            if (worldRank < 100 && null != tmp)
            {
                tmp.text = $"世界第{NumberToChinese(worldRank + 1)}";
            }
        }

        //数字1-9转换为中文数字
        public string OneBitNumberToChinese(string num)
        {
            string numStr = "123456789";
            string chineseStr = "一二三四五六七八九";
            string result = "";
            int numIndex = numStr.IndexOf(num);
            if (numIndex > -1)
            {
                result = chineseStr.Substring(numIndex, 1);
            }
            return result;
        }
        //阿拉伯数字转换为中文数字（0-99999）
        public string NumberToChinese(int num)
        {
            string strNum = num.ToString();
            string result = "";
            if (strNum.Length == 5)
            {//万
                result += OneBitNumberToChinese(strNum.Substring(0, 1)) + "万";
                strNum = strNum.Substring(1);
            }
            if (strNum.Length == 4)
            {//千
                string secondBitNumber = strNum.Substring(0, 1);
                if (secondBitNumber == "0") result += "零";
                else result += OneBitNumberToChinese(secondBitNumber) + "千";
                strNum = strNum.Substring(1);
            }
            if (strNum.Length == 3)
            {//百
                string hundredBitNumber = strNum.Substring(0, 1);
                if (hundredBitNumber == "0" && result.Substring(result.Length - 1) != "零") result += "零";
                else result += OneBitNumberToChinese(hundredBitNumber) + "百";
                strNum = strNum.Substring(1);
            }
            if (strNum.Length == 2)
            {//十
                string hundredBitNumber = strNum.Substring(0, 1);
                if (hundredBitNumber == "0" && result.Substring(result.Length - 1) != "零") result += "零";
                else if (hundredBitNumber == "1" && string.IsNullOrEmpty(result)) result += "十";//10->十
                else result += OneBitNumberToChinese(hundredBitNumber) + "十";
                strNum = strNum.Substring(1);
            }
            if (strNum.Length == 1)
            {//个
                if (strNum == "0") result += "零";
                else result += OneBitNumberToChinese(strNum);
            }
            //100->一百零零
            if (!string.IsNullOrEmpty(result) && result != "零") result = result.TrimEnd('零');
            return result;
        }
    }
}
