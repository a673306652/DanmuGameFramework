using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Daan;

namespace DanMuGame
{
    public class CarouselUISystem : MonoBehaviour
    {
        public RectTransform parent;
        private CarouselUIPanel activePanel;
        private List<CarouselUIPanel> list = new List<CarouselUIPanel>();

        private Tweener exitTweener;
        private Tweener enterTweener;

        private void Awake()
        {
            this.list.AddRange(this.parent.GetComponentsInChildren<CarouselUIPanel>());
            foreach (var item in this.list)
            {
                item.system = this;
                item.gameObject.SetActive(false);
            }
            this.MoveNext();
        }

        public bool MoveNext()
        {
            var next = this.GetNext(this.activePanel);
            if (next)
            {
                if (this.activePanel != null)
                {
                    this.PanelExit(this.activePanel);
                }
                this.activePanel = next;
                this.activePanel.transform.SetAsFirstSibling();
                this.activePanel.gameObject.SetActive(true);
                this.list.Remove(this.activePanel);
                this.list.Add(this.activePanel);
                this.PanelEnter(this.activePanel);
                return true;
            }

            return false;
        }

        public CarouselUIPanel GetNext(CarouselUIPanel active)
        {
            CarouselUIPanel result = null;
            for (int i = 0; i < this.list.Count; i++)
            {
                if (this.list[i].insert)
                {
                    result = this.list[i];
                    break;
                }
                if (this.list[i] == active || !this.list[i].CheckCD())
                {
                    continue;
                }
                else
                {
                    result = this.list[i];
                    break;
                }
            }
            if (result != null)
            {
                result.insert = false;
            }
            return result;
        }

        void PanelExit(CarouselUIPanel panel)
        {
            this.exitTweener?.Kill(true);
            this.exitTweener = panel.rect.DOAnchorPosX(-this.parent.sizeDelta.x, 0.5F).OnComplete(() =>
            {
                panel.gameObject.SetActive(false);
                if (panel.once)
                {
                    ResourceManager.Instance.Despawn(panel);
                    this.list.Remove(panel);
                }
            });
        }

        void PanelEnter(CarouselUIPanel panel)
        {
            this.enterTweener?.Kill(true);
            panel.rect.anchoredPosition = new Vector2(this.parent.sizeDelta.x, 0);
            this.enterTweener = panel.rect.DOAnchorPosX(0, 0.5F).OnComplete(() =>
            {

            });
        }
    }



}
