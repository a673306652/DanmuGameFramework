using System.Collections;
using System.Collections.Generic;
using Daan;
using DanMuGame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DanMuGame
{
    public class Notice_Banner : Notice_User
    {
        [SerializeField] private Text text;

        private UnityAction onComplete;

        public void SetContent(string content)
        {
            this.text.text = content;
        }

        public void SetOnComplete(UnityAction oc)
        {
            this.onComplete = oc;
        }

        public void OnComplete()
        {
            onComplete?.Invoke();
            onComplete = null;
        }
    }
}