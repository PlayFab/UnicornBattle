using UnityEngine.Events;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Slider))]
    [AddComponentMenu("UI/Tweening/Slider")]
    public class TweenSlider : TweenMain
    {
        public float
            from,
            to;

        private float
            _from,
            _to;

        private Slider _sld;
        public Slider Sld
        {
            get { return _sld ?? (_sld = GetComponent<Slider>()); }
        }

        public float value
        {
            get { return Sld.value; }
            set { Sld.value = value; }
        }

        protected override void Start()
        {
            if (fromOffset) _from = value + from;
            else _from = from;
            if (toOffset) _to = value + to;
            else _to = to;
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Mathf.Lerp(_from, _to, factor);
        }

        public override void ToCurrentValue() { to = value; }
        public override void FromCurrentValue() { from = value; }

        /// <summary>
        /// Create a TweenSlider Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween to</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="value">The ending value for the tween</param>
        /// <param name="finished">A optional callback to fire when the tween is done</param>
        /// <returns>Returns a reference to the new TweenAlpha component</returns>
        public static TweenSlider Tween(GameObject go, float duration, float value, UnityAction finished = null)
        {
            TweenSlider cls = TweenMain.Tween<TweenSlider>(go, duration, finished);
            cls.from = cls.value;
            cls.to = value;
            cls.Start();
            return cls;
        }

        /// <summary>
        /// Create a TweenSlider Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween to</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="finished">A optional callback to fire when the tween is done</param>
        /// <returns>Returns a reference to the new TweenAlpha component</returns>
        public static TweenSlider Tween(GameObject go, float duration, float fromVal, float toVal,
            UnityAction finished = null)
        {
            return Tween(go, duration, fromVal, toVal, Style.Once, Method.Linear, finished);
        }

        /// <summary>
        /// Create a TweenSlider Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween to</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="style">The style of tween (Once, Looped, PingPong)</param>
        /// <param name="method">The Interpolation method of the tween</param>
        /// <param name="finished">A optional callback to fire when the tween is done</param>
        /// <returns>Returns a reference to the new TweenAlpha component</returns>
        public static TweenSlider Tween(GameObject go, float duration, float fromVal, float toVal,
            Style style, Method method, UnityAction finished = null)
        {
            TweenSlider cls = TweenMain.Tween<TweenSlider>(go, duration, style, method, finished);
            cls.from = fromVal;
            cls.to = toVal;
            cls.Start();
            return cls;
        }
    }
}
