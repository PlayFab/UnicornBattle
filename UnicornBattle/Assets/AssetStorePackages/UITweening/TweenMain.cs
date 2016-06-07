using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public abstract class TweenMain : MonoBehaviour
    {
        static public TweenMain self;

        #region Enums
        public enum Method
        {
            Linear,
            EaseIn,
            EaseOut,
            EaseInOut,
            BounceIn,
            BounceOut,
            BounceInOut
        }

        public enum Style
        {
            Once,
            Loop,
            PingPong
        }
        #endregion

        #region publicVars
        public UnityEvent OnFinished;

        public bool fromOffset = false;
        public bool toOffset = false;

        public Method method = Method.Linear;
        public AnimationCurve functionCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        public Style style = Style.Once;
        public bool ignoreTimeScale = true;
        public float delay = 0f;
        public float duration = 1f;

        public float amountPerDelta
        {
            get
            {
                if (_Duration != duration)
                {
                    _Duration = duration;
                    _AmountPerDelta = Mathf.Abs((duration > 0f) ? 1f / duration : 1000f);
                }
                return _AmountPerDelta;
            }
        }

        public float tweenFactor { get { return _Factor; } set { _Factor = Mathf.Clamp01(value); } }
        #endregion

        #region privateVars
        private bool _Started = false;
        private float
            _StartTime = 0f,
            _Duration = 0f,
            _AmountPerDelta = 1000f,
            _Factor = 0f;
        #endregion

        void Reset()
        {
            if (!_Started)
            {
                ToCurrentValue();
                FromCurrentValue();
            }
            Start();
        }

        void Awake()
        {
            if (OnFinished == null)
                OnFinished = new UnityEvent();
        }

        protected virtual void Start() { Update(); }
        void Update()
        {
            float delta = ignoreTimeScale ? UnScaledTime.deltaTime : Time.deltaTime;
            float time = ignoreTimeScale ? UnScaledTime.time : Time.time;
            if (!_Started)
            {
                _Started = true;
                _StartTime = time + delay;
            }

            if (time < _StartTime)
                return;

            _Factor += amountPerDelta * delta;

            if (style == Style.Loop)
            {
                if (_Factor > 1f)
                    _Factor -= Mathf.Floor(_Factor);
            }
            else if (style == Style.PingPong)
            {
                if (_Factor > 1f)
                {
                    _Factor = 1f - (_Factor - Mathf.Floor(_Factor));
                    _AmountPerDelta = -_AmountPerDelta;
                }
                else if (_Factor < 0f)
                {
                    _Factor = -_Factor;
                    _Factor -= Mathf.Floor(_Factor);
                    _AmountPerDelta = -_AmountPerDelta;
                }
            }

            if ((style == Style.Once) && (duration == 0f || _Factor > 1f || _Factor < 0f))
            {
                _Factor = Mathf.Clamp01(_Factor);
                Sample(_Factor, true);

                if (duration == 0f || (_Factor == 1f && _AmountPerDelta > 0f || _Factor == 0f && amountPerDelta < 0f))
                    enabled = false;

                //Event Callback stuff
                if (self == null)
                {
                    self = this;
                    if (OnFinished != null)
                        OnFinished.Invoke();
                }
                self = null;
            }
            else
                Sample(_Factor, false);
        }

        void OnDisable() { _Started = false; }

        protected void Sample(float factor, bool isFinished)
        {
            float val = Mathf.Clamp01(factor);

            switch (method)
            {
                case Method.Linear:
                    val = Easing.Linear(val);
                    break;

                case Method.EaseIn:
                    val = Easing.Sinusoidal.In(val);
                    break;

                case Method.EaseOut:
                    val = Easing.Sinusoidal.Out(val);
                    break;

                case Method.EaseInOut:
                    val = Easing.Sinusoidal.InOut(val);
                    break;

                case Method.BounceIn:
                    val = Easing.Bounce.In(val);
                    break;

                case Method.BounceOut:
                    val = Easing.Bounce.Out(val);
                    break;

                case Method.BounceInOut:
                    val = Easing.Bounce.InOut(val);
                    break;
            }
            OnUpdate((functionCurve != null) ? functionCurve.Evaluate(val) : val, isFinished);
        }

        #region PlayMethods
        /// <summary>
        /// Reset Tween to begining.
        /// </summary>
        public void ResetToBeginning()
        {
            _Started = false;
            _Factor = (_AmountPerDelta < 0f) ? 1f : 0f;
            Sample(_Factor, false);
            Start();
        }

        /// <summary>
        /// Plays Tween.
        /// </summary>
        public void Play() { PlayForward(); }

        /// <summary>
        /// Plays Tween.
        /// </summary>
        public void PlayForward()
        {
            _AmountPerDelta = Mathf.Abs(_AmountPerDelta);
            enabled = true;
            Start();
            Update();
        }

        /// <summary>
        /// Plays Tween in reverse.
        /// </summary>
        public void PlayReverse()
        {
            _AmountPerDelta = -Mathf.Abs(_AmountPerDelta);
            enabled = true;
            Start();
            Update();
        }

        /// <summary>
        /// Toogle play direction and play tween.
        /// </summary>
        public void Toggle()
        {
            if (_Factor > 0f)
                _AmountPerDelta = -amountPerDelta;
            else
                _AmountPerDelta = Mathf.Abs(amountPerDelta);
            enabled = true;
        }
        #endregion

        /// <summary>
        /// Interface for new tweeners to implemeant.
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="isFinished"></param>
        abstract protected void OnUpdate(float factor, bool isFinished);
        
        /// <summary>
        /// Set "To" the current Value.
        /// </summary>
        public virtual void ToCurrentValue() { }
        /// <summary>
        /// Set "From" the current Value.
        /// </summary>
        public virtual void FromCurrentValue() { }

        protected static T Tween<T>(GameObject go, float duration,
            Style style, Method method, UnityAction finished = null) where T :TweenMain
        {
            T cls = Tween<T>(go, duration, finished);
            cls.style = style;
            cls.method = method;
            return cls;
        }

        protected static T Tween<T>(GameObject go, float duration,
            UnityAction finished = null) where T : TweenMain
        {
            T cls = go.GetComponent<T>();
            if (cls == null)
                cls = go.AddComponent<T>();

            cls.OnFinished.RemoveAllListeners();
            if (finished != null)
                cls.OnFinished.AddListener(finished);

            cls._Started = false;
            cls.duration = duration;
            cls._Factor = 0f;
            cls._AmountPerDelta = Mathf.Abs(cls._AmountPerDelta);
            cls.functionCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 1f, 1f));
            cls.enabled = true;

            if (duration <= 0f)
            {
                cls.Sample(1f, true);
                cls.enabled = false;
            }
            return cls;
        }
    }
}
