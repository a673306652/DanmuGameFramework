using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
using System;
using DG.Tweening;

namespace DanMuGame
{
    public class CameraManager : Manager<CameraManager>
    {
        public bool focusing = false;
        private List<Action> focusQueue = new List<Action>();
        private Sequence cameraTweener;
        public Vector3 cameraOffset;
        public float defaultCameraSize = 5.4F;
        public float focusCameraSize = 3;

        private Tweener shakeTweener;

        private new Camera camera;
        private Transform root;

        public override void Init()
        {
            base.Init();
            this.camera = Camera.main;
            this.root = this.camera.transform.parent;
        }

        private void Update()
        {
            if (!this.focusing && this.focusQueue.Count > 0)
            {
                this.focusQueue[0].Invoke();
                this.focusQueue.RemoveAt(0);
            }
        }

        public void AddFocusQueue(Action action)
        {
            this.focusQueue.Add(action);
        }

        public void Focus(Vector3 point, Action endCall = null, Action startCall = null, float time = 1.5F, float speed = 0.5F)
        {
            this.focusing = true;
            var originCamera = this.FocusOn(point, startCall, speed);

            Timer.Register(time, () =>
            {
                this.focusing = false;
                endCall?.Invoke();

                this.FocusOff(speed);
            });
        }

        public void FocusOff(float speed = 0.5F)
        {
            this.cameraTweener?.Kill();
            this.cameraTweener = DOTween.Sequence();
            this.cameraTweener.Join(this.root.transform.DOMove(Vector3.zero, speed).SetEase(Ease.Linear));
            this.cameraTweener.Join(this.camera.DOOrthoSize(this.defaultCameraSize, speed).SetEase(Ease.Linear));
        }

        public (Vector3, float) FocusOn(Vector3 point, Action startCall = null, float speed = 0.5F)
        {
            var originSize = this.camera.orthographicSize;
            var originPos = this.camera.transform.position;
            this.cameraTweener?.Kill();
            this.cameraTweener = DOTween.Sequence();
            this.cameraTweener.Join(this.root.transform.DOMove(point + this.cameraOffset, speed).SetEase(Ease.Linear));
            this.cameraTweener.Join(this.camera.DOOrthoSize(this.focusCameraSize, speed).SetEase(Ease.Linear));
            this.cameraTweener.OnComplete(() =>
            {
                startCall?.Invoke();
            });
            return (originPos, originSize);
        }

        public Tweener Shake(float duration, Vector3 range, int vibrato = 10)
        {
            this.shakeTweener?.Kill(true);
            this.shakeTweener = this.camera.DOShakePosition(duration, strength: range, fadeOut: false, vibrato: vibrato);
            return this.shakeTweener;
        }

        public void Clear()
        {
            this.focusQueue.Clear();
            this.focusing = false;
            this.cameraTweener?.Kill(true);
        }

    }
}
