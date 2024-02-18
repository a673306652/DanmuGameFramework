using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
using DanMuGame;
using UnityEngine.UI;
using DG.Tweening;

namespace DanMuGame
{
    public class Notice_Score : Notice_User
    {
        private Text scoreTxt;
        private Sequence tw;
        public override void Init()
        {
            base.Init();
            this.scoreTxt = this.transform.Find<Text>("Root/Score");
            this.tw = DOTween.Sequence();
            this.tw.SetEase(Ease.Linear);
        }

        public void SetScore(int score)
        {
            this.scoreTxt.text = "0";
            this.tw?.Kill(true);
            var punch = this.scoreTxt.transform.DOPunchScale(Vector3.one * 0.1F, 0.1F).SetLoops(-1);
            this.tw.Join(punch);
            this.tw.Join(DOTween.To((float value) =>
            {
                this.scoreTxt.text = ((int)value).ToString();
            }, 0, score, 2).OnComplete(() =>
            {
                punch.Kill(true);
                this.scoreTxt.transform.DOPunchScale(Vector3.one * 0.1F, 0.1F);
                this.tw.Kill(true);
            }));
        }
    }
}
