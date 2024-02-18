using System;
using System.Collections;
using System.Collections.Generic;
using Daan;
using DG.Tweening;
using UnityEngine;

namespace DanMuGame
{
    public class CameraSync : MonoBehaviour
    {
        public Camera Target;

        private new Camera camera;

        void Awake()
        {
            this.camera = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            this.camera.orthographicSize = Target.orthographicSize;
        }
    }
}