using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Tweening/Rotation")]
    public class TweenRot : TweenMain
    {
        public Vector3
            from,
            to;

        private Vector3
            _from,
            _to;

        private RectTransform _rect;
        private RectTransform Rect
        {
            get { return _rect ?? (_rect = GetComponent<RectTransform>()); }
        }

        public Quaternion value
        {
            get { return Rect.localRotation; }
            set { Rect.localRotation = value; }
        }

        protected override void Start()
        {
            if (fromOffset) _from = value.eulerAngles + from;
            else _from = from;
            if (toOffset) _to = value.eulerAngles + to;
            else _to = to;
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Quaternion.Euler(Vector3.Lerp(_from, _to, factor));
        }

        public override void ToCurrentValue() { to = value.eulerAngles; }
        public override void FromCurrentValue() { from = value.eulerAngles; }

        /// <summary>
        /// Create a TweenRot Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="rot">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenRot component</returns>
        public static TweenRot Tween(GameObject go, float duration, Quaternion rot, UnityAction finished = null)
        {
            TweenRot cls = TweenMain.Tween<TweenRot>(go, duration, finished);
            cls.from = cls.value.eulerAngles;
            cls.to = rot.eulerAngles;
            cls.Start();
            return cls;
        }

        /// <summary>
        /// Create a TweenRot Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenRot component</returns>
        public static TweenRot Tween(GameObject go, float duration, Quaternion fromVal, Quaternion toVal,
            UnityAction finished = null)
        {
            return Tween(go, duration, fromVal, toVal, Style.Once, Method.Linear, finished);
        }

        /// <summary>
        /// Create a TweenRot Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="style">The style of tween (Once, Looped, PingPong)</param>
        /// <param name="method">The Interpolation method of the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <returns>Return reference to the new TweenRot component</returns>
        public static TweenRot Tween(GameObject go, float duration, Quaternion fromVal, Quaternion toVal,
            Style style, Method method, UnityAction finished = null)
        {
            TweenRot cls = TweenMain.Tween<TweenRot>(go, duration, style, method, finished);
            cls.from = fromVal.eulerAngles;
            cls.to = toVal.eulerAngles;
            cls.Start();
            return cls;
        }
    }
}
