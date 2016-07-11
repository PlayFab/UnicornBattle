using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("UI/Tweening/Canvas Group Alpha")]
    public class TweenCGAlpha : TweenMain
    {

        [Range(0f, 1f)]
        public float
            from,
            to;

        private float
            _from,
            _to;

        private CanvasGroup _mcg;
        private CanvasGroup _cg
        {
            get { return _mcg ?? (_mcg = GetComponent<CanvasGroup>()); }
        }

        public float value
        {
            get { return _cg.alpha; }
            set { _cg.alpha = value; }
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
        /// Create a TweenCGAlpha Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="alpha">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenCGAlpha component</returns>
        public static TweenCGAlpha Tween(GameObject go, float duration, float alpha, UnityAction finished = null)
        {
            TweenCGAlpha cls = TweenMain.Tween<TweenCGAlpha>(go, duration, finished);
            cls.from = cls.value;
            cls.to = alpha;
            cls.Start();
            return cls;
        }

        /// <summary>
        /// Create a TweenCGAlpha Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenCGAlpha component</returns>
        public static TweenCGAlpha Tween(GameObject go, float duration, float fromVal, float toVal,
            UnityAction finished = null)
        {
            return Tween(go, duration, fromVal, toVal, Style.Once, Method.Linear, finished);
        }

        /// <summary>
        /// Create a TweenCGAlpha Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="style">The style of tween (Once, Looped, PingPong)</param>
        /// <param name="method">The Interpolation method of the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenCGAlpha component</returns>
        public static TweenCGAlpha Tween(GameObject go, float duration, float fromVal, float toVal,
            Style style, Method method, UnityAction finished = null)
        {
            TweenCGAlpha cls = TweenMain.Tween<TweenCGAlpha>(go, duration, style, method, finished);
            cls.from = fromVal;
            cls.to = toVal;
            cls.Start();
            return cls;
        }
    }
}
