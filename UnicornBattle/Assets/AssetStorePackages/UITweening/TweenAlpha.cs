using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("UI/Tweening/Alpha")]
    public class TweenAlpha : TweenMain
    {
        [Range(0f, 1f)]
        public float
            from,
            to;

        private float
            _from,
            _to;

        private Graphic _gfx;
        private Graphic Gfx
        {
            get { return _gfx ?? (_gfx = GetComponent<Graphic>()); }
        }

        public float value
        {
            get { return Gfx.color.a; }
            set { Gfx.color = new Color(Gfx.color.r, Gfx.color.g, Gfx.color.b, value); }
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
        /// Create a TweenAlpha Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="alpha">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenAlpha component</returns>
        public static TweenAlpha Tween(GameObject go, float duration, float alpha, UnityAction finished = null)
        {
            TweenAlpha cls = TweenMain.Tween<TweenAlpha>(go, duration, finished);
            cls.from = cls.value;
            cls.to = alpha;
            cls.Start();
            return cls;
        }

        /// <summary>
        /// Create a TweenAlpha Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenAlpha component</returns>
        public static TweenAlpha Tween(GameObject go, float duration, float fromVal, float toVal, UnityAction finished = null)
        {
            return Tween(go, duration, fromVal, toVal, Style.Once, Method.Linear, finished);
        }

        /// <summary>
        /// Create a TweenAlpha Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="style">The style of tween (Once, Looped, PingPong)</param>
        /// <param name="method">The Interpolation method of the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenAlpha component</returns>
        public static TweenAlpha Tween(GameObject go, float duration, float fromVal, float toVal,
            Style style, Method method, UnityAction finished = null)
        {
            TweenAlpha cls = TweenMain.Tween<TweenAlpha>(go, duration, style, method, finished);
            cls.from = fromVal;
            cls.to = toVal;
            cls.Start();
            return cls;
        }
    }
}
