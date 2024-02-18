using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
using DanMuGame;
using UnityEngine.UI;
using DG.Tweening;

namespace DanMuGame
{
    public abstract class Notice_Custom : Notice_User
    {
        public abstract void SetParams(object[] p);
    }
}
