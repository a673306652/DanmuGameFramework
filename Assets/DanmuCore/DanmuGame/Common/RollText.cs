using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Daan;
using DG.Tweening;

namespace DanMuGame
{
    [RequireComponent(typeof(Text))]
    public class RollText : MonoBehaviour
    {
        public bool moveLeft = true;
        public float speed;
        public int copyCount = 2;
        private Text txt;
        private Vector2 originPos;
        private float originWidth;

        private void Awake()
        {
            this.txt = this.GetComponent<Text>();
            this.originPos = this.txt.rectTransform.anchoredPosition;
            this.originWidth = this.txt.preferredWidth;
            var content = txt.text;

            var childCount = this.txt.transform.childCount;
            for (int i = 0; i < this.copyCount - 1; i++)
            {
                txt.text += content;
                for (int j = 0; j < childCount; j++)
                {
                    var obj = GameObject.Instantiate(this.txt.transform.GetChild(j).gameObject);
                    obj.transform.SetParent(this.txt.transform);
                    obj.GetRectTransform().anchoredPosition = this.txt.transform.GetChild(j).GetRectTransform().anchoredPosition + Vector2.right * this.originWidth * (i + 1);
                }
            }
        }

        private void Start()
        {
            this.txt.rectTransform.DOAnchorPos(this.originPos + new Vector2((this.moveLeft ? -1 : 1) * this.originWidth, 0), this.speed).SetEase(Ease.Linear).SetSpeedBased().SetLoops(-1, LoopType.Restart);
        }
    }
}
