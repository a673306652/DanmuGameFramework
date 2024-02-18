namespace Modules.Timer
{
    using UnityEngine;
    using UnityEngine.Events;

    public class Timer : MonoBehaviour
    {
        // 延迟时间(秒)
        [SerializeField]
        private float _Delay = 0;

        // 间隔时间(秒)
        [SerializeField]
        private float _Interval = 1;

        // 重复次数
        [SerializeField]
        private int _RepeatCount = 1;

        // 自动计时
        [SerializeField]
        private bool _AutoStart = false;

        // 自动销毁
        [SerializeField]
        private bool _AutoDestory = false;

        // 计时间隔
        public UnityEvent OnIntervalEvent;

        // 计时完成
        public UnityEvent OnCompleteEvent;

        // 回调事件代理
        public delegate void TimerCallback(Timer timer);

        // 当前时间
        private float currentTime = 0;

        // 当前次数
        private int currentCount = 0;

        // 上一次间隔时间
        private float lastTime = 0;

        private float deltaTime = 0;

        // 计时间隔
        private TimerCallback onIntervalCall;

        // 计时结束
        private TimerCallback onCompleteCall;

        public float DeltaTime
        {
            get
            {
                return deltaTime;
            }
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }
        }

        void Start()
        {
            enabled = _AutoStart;
        }

        void FixedUpdate()
        {
            if (!enabled) return;
            AddInterval(Time.deltaTime);
        }

        /// <summary>
        /// 增加间隔时间
        /// </summary>
        /// <param name="dt"></param>
        private void AddInterval(float dt)
        {
            currentTime += dt;
            if (currentTime < _Delay) return;
            if (currentTime - lastTime >= _Interval)
            {
                deltaTime = currentTime - lastTime;
                currentCount++;
                lastTime = currentTime;
                if (_RepeatCount <= 0)
                {
                    // 无限重复
                    if (currentCount == int.MaxValue) Reset();
                    onIntervalCall?.Invoke(this);
                    OnIntervalEvent?.Invoke();
                }
                else
                {
                    if (currentCount < _RepeatCount)
                    {
                        // 计时间隔
                        onIntervalCall?.Invoke(this);
                        OnIntervalEvent?.Invoke();
                    }
                    else
                    {
                        // 计时结束
                        SwitchOff();
                        onCompleteCall?.Invoke(this);
                        OnCompleteEvent?.Invoke();
                        if (_AutoDestory && !enabled) Destroy(this);
                    }
                }
            }
        }

        /// <summary>
        /// 开始/继续计时
        /// </summary>
        public void SwitchOn()
        {
            enabled = _AutoStart = true;
        }

        /// <summary>
        /// 开始计时
        /// </summary>
        /// <param name="time">时间(秒)</param>
        /// <param name="onComplete">计时完成回调事件</param>
        public void SwitchOn(float time, TimerCallback onComplete)
        {
            SwitchOn(time, 1, null, onComplete);
        }

        /// <summary>
        /// 开始计时
        /// </summary>
        /// <param name="interval">计时间隔</param>
        /// <param name="repeatCount">重复次数</param>
        /// <param name="onComplete">计时完成回调事件</param>
        public void SwitchOn(
            float interval,
            int repeatCount,
            TimerCallback onComplete
        )
        {
            SwitchOn(interval, repeatCount, null, onComplete);
        }

        /// <summary>
        /// 开始计时
        /// </summary>
        /// <param name="interval">计时间隔</param>
        /// <param name="repeatCount">重复次数</param>
        /// <param name="onInterval">计时间隔回调事件</param>
        /// <param name="onComplete">计时完成回调事件</param>
        public void SwitchOn(
            float interval,
            int repeatCount,
            TimerCallback onInterval,
            TimerCallback onComplete
        )
        {
            this._Interval = interval;
            this._RepeatCount = repeatCount;
            onIntervalCall = onInterval;
            onCompleteCall = onComplete;
            Reset();
            enabled = _AutoStart = true;
        }

        public void SwitchOn(
            TimerCallback onInterval,
            TimerCallback onComplete = null
        )
        {
            onIntervalCall = onInterval;
            onCompleteCall = onComplete;
            Reset();
            enabled = _AutoStart = true;
        }

        /// <summary>
        /// 暂停计时
        /// </summary>
        public void SwitchOff()
        {
            enabled = _AutoStart = false;
        }

        /// <summary>
        /// 停止Timer并重置数据
        /// </summary>
        public void Reset()
        {
            lastTime = currentTime = deltaTime = currentCount = 0;
        }

        /// <summary>
        /// 重置数据并重新开始计时
        /// </summary>
        public void Restart()
        {
            Reset();
            SwitchOn();
        }
    }

}