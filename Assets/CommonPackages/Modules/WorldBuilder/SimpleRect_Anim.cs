using UnityEngine;

namespace Modules.WorldBuilder
{
    [ExecuteInEditMode]
    public class SimpleRect_Anim : MonoBehaviour
    {
        public Transform LU;
        public Transform RU;
        public Transform LD;
        public Transform RD;
        public Transform Center;

        public float Width = 2f;
        public float Height = 2f;
        public float CenterScale = 1f;
        public float RectScale = 1f;

        [SerializeField]
        private bool _IsPingPong;
        [SerializeField]
        private bool _CenterPingPong;
        [SerializeField]
        private bool _GlobalPingPong;
        [SerializeField]
        private float _PingPongSpeed;
        [SerializeField]
        private float _PingPongAddtion;

        [Range(0, 1)]
        public float YScale = 1f;
        public DynamicStyle Style;
        public enum DynamicStyle { Pingpong, Sine }
        private float timeTick;

        void Update()
        {
            timeTick += Time.deltaTime * _PingPongSpeed;
            var sinTime = 0f;
            if (Style == DynamicStyle.Pingpong)
            {
                sinTime = Mathf.PingPong(timeTick, 1) * 2 - 1;
            }

            if (Style == DynamicStyle.Sine)
            {
                sinTime = Mathf.Sin(timeTick);
            }
            if (LU != null)
            {
                LU.localPosition = new Vector3(-Width / 2, 0, Height / 2);
                if (!_IsPingPong)
                    LU.localScale = Vector3.one * RectScale;
                if (_IsPingPong && !_GlobalPingPong) LU.localScale = Vector3.one * RectScale;
            }
            if (RU != null)
            {
                RU.localPosition = new Vector3(Width / 2, 0, Height / 2);
                if (!_IsPingPong)
                    RU.localScale = Vector3.one * RectScale;
                if (_IsPingPong && !_GlobalPingPong) RU.localScale = Vector3.one * RectScale;
            }
            if (LD != null)
            {
                LD.localPosition = new Vector3(-Width / 2, 0, -Height / 2);
                if (!_IsPingPong)
                    LD.localScale = Vector3.one * RectScale;
                if (_IsPingPong && !_GlobalPingPong) LD.localScale = Vector3.one * RectScale;

            }
            if (RD != null)
            {
                RD.localPosition = new Vector3(Width / 2, 0, -Height / 2);
                if (!_IsPingPong)
                    RD.localScale = Vector3.one * RectScale;
                if (_IsPingPong && !_GlobalPingPong) RD.localScale = Vector3.one * RectScale;

            }

            if (Center != null && !_IsPingPong)
            {
                Center.transform.localScale = Vector3.one * CenterScale;
            }

            if (_IsPingPong)
            {
                if (_CenterPingPong && null != Center)
                {
                    if (_GlobalPingPong)
                    {
                        transform.localScale = Vector3.one + Vector3.one * (sinTime * _PingPongAddtion);
                    }
                    else
                    {
                        transform.localScale = Vector3.one;
                        Center.transform.localScale = Vector3.one * CenterScale + Vector3.one * (sinTime * _PingPongAddtion);
                    }
                }
            }

            transform.localScale = new Vector3(YScale <= 0 ? 0 : transform.localScale.x, YScale <= 0 ? 0 : transform.localScale.y * YScale, YScale <= 0 ? 0 : transform.lossyScale.z);
        }
    }
}