using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Tweening/Scale")]
    public class TweenScale : TweenMain
    {
        public Vector3
            from = Vector3.one,
            to = Vector3.one;

        private Vector3
            _from = Vector3.one,
            _to = Vector3.one;

        private RectTransform _rect;
        private RectTransform Rect
        {
            get { return _rect ?? (_rect = GetComponent<RectTransform>()); }
        }

        public Vector3 value
        {
            get { return Rect.localScale; }
            set { Rect.localScale = value; }
        }

        protected override void Start()
        {
            if (fromOffset) _from = value + from;
            else _from = from;
            if (toOffset) _to = value + to;
            else _to = to;
            base.Start();
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Vector3.Lerp(_from, _to, factor);
        }

        public override void FromCurrentValue() { from = value; }
        public override void ToCurrentValue() { to = value; }

        /// <summary>
        /// Create a TweenScale Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="scale">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenScale component</returns>
        public static TweenScale Tween(GameObject go, float duration, Vector3 scale, UnityAction finished = null)
        {
            TweenScale cls = TweenMain.Tween<TweenScale>(go, duration, finished);
            cls.from = cls.value;
            cls.to = scale;
            cls.Start();
            return cls;
        }

        /// <summary>
        /// Create a TweenScale Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenScale component</returns>
        public static TweenScale Tween(GameObject go, float duration, Vector3 fromVal, Vector3 toVal,
            UnityAction finished = null)
        {
            return Tween(go, duration, fromVal, toVal, Style.Once, Method.Linear, finished);
        }

        /// <summary>
        /// Create a TweenScale Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="style">The style of tween (Once, Looped, PingPong)</param>
        /// <param name="method">The Interpolation method of the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenScale component</returns>
        public static TweenScale Tween(GameObject go, float duration, Vector3 fromVal, Vector3 toVal,
            Style style, Method method, UnityAction finished = null)
        {
            TweenScale cls = TweenMain.Tween<TweenScale>(go, duration, style, method, finished);
            cls.from = fromVal;
            cls.to = toVal;
            cls.Start();
            return cls;
        }
    }
}
