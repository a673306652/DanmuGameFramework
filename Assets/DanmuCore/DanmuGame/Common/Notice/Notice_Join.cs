using System;
using System.Collections;
using System.Collections.Generic;
using Daan;
using DanMuGame;
using UnityEngine;
using UnityEngine.Events;

namespace DanMuGame
{
    public class Notice_Join : Notice_User
    {
        private UnityAction onComplete;
        public NoticeItem item;
        public Animator anim;
        public void Play()
        {
            anim.Play("Open");
        }

        private void FixedUpdate()
        {
            autoDespawnTimer += 1f;
            if (autoDespawnTimer>10)
            {
                autoDespawnTimer = 1f;
            }
        }

        public void SetOnComplete(UnityAction oc)
        {
            this.onComplete = oc;
        }
        
        public void OnComplete()
        {
            var temp = this.onComplete;
            temp?.Invoke();
            this.onComplete = null;
        }
    }
}
