using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("UI/Tweening/Color")]
    public class TweenColor : TweenMain
    {
        public Color from = Color.black;
        public Color to = Color.black;

        private Graphic _gfx;
        private Graphic Gfx
        {
            get { return _gfx ?? (_gfx = GetComponent<Graphic>()); }
        }

        public Color value
        {
            get { return Gfx.color; }
            set { Gfx.color = value; }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Color.Lerp(from, to, factor);
        }

        public override void ToCurrentValue() { to = value; }
        public override void FromCurrentValue() { from = value; }

        /// <summary>
        /// Create a TweenColor Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="color">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenColor component</returns>
        public static TweenColor Tween(GameObject go, float duration, Color color, UnityAction finished = null)
        {
            TweenColor cls = TweenMain.Tween<TweenColor>(go, duration, finished);
            cls.from = cls.value;
            cls.to = color;
            cls.Start();
            return cls;
        }

        /// <summary>
        /// Create a TweenColor Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenColor component</returns>
        public static TweenColor Tween(GameObject go, float duration, Color fromVal, Color toVal, UnityAction finished = null)
        {
            return Tween(go, duration, fromVal, toVal, Style.Once, Method.Linear, finished);
        }

        /// <summary>
        /// Create a TweenColor Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="style">The style of tween (Once, Looped, PingPong)</param>
        /// <param name="method">The Interpolation method of the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenColor component</returns>
        public static TweenColor Tween(GameObject go, float duration, Color fromVal, Color toVal,
            Style style, Method method, UnityAction finished = null)
        {
            TweenColor cls = TweenMain.Tween<TweenColor>(go, duration, style, method, finished);
            cls.from = fromVal;
            cls.to = toVal;
            cls.Start();
            return cls;
        }
    }
}
