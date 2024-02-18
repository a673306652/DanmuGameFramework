using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Daan;
using DanMuGame;
using Hisao;
using UnityEngine.EventSystems;

namespace DanMuGame
{
    public class DragableUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IScrollHandler
    {
        private Vector3 originPos;
        private Vector3 originScale;
        private Vector3 tempPos;
        private Vector3 tempMousePos;
        private void Awake()
        {
            this.originPos = this.transform.localPosition;
            this.originScale = this.transform.localScale;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            tempPos = this.transform.position;
            tempMousePos = HisaoTransform.Screen2WorldPos(Input.mousePosition, Camera.main, 0);
            //this.transform.localPosition = eventData.position - new Vector2(Screen.width / 2, Screen.height / 2);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var dir = (HisaoTransform.Screen2WorldPos( Input.mousePosition,Camera.main,0)  - tempMousePos.xy()).normalized;

            this.transform.position = tempPos + dir * Vector3.Distance(tempMousePos, HisaoTransform.Screen2WorldPos( Input.mousePosition,Camera.main,0));
            //  this.transform.localPosition = eventData.position - new Vector2(Screen.width / 2, Screen.height / 2);
        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                this.transform.localPosition = this.originPos;
                this.transform.localScale = this.originScale;
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            var d = Mathf.Max(this.transform.localScale.x + 0.1F * eventData.scrollDelta.y, 0.1F);
            this.transform.localScale = Vector3.one * d;
        }
    }
}
