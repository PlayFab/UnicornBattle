using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Tweening/Position")]
    public class TweenPos : TweenMain 
    {
        public Space CSpace = Space.World;

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

        public Vector3 value
        {
            get
            {
                if (CSpace == Space.Self) return Rect.localPosition;
                else return Rect.position;
            }
            set
            {
                if (CSpace == Space.Self) Rect.localPosition = value;
                else Rect.position = value;
            }
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
            value = Vector3.Lerp(_from, _to, factor);
        }

        public override void ToCurrentValue() { to = value; }
        public override void FromCurrentValue() { from = value; }

        /// <summary>
        /// Create a TweenPos Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="pos">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <param name="cSpace">A optional Arugmeant to define the coordnaite space to work in</param>
        /// <returns>Return reference to the new TweenPos component</returns>
        public static TweenPos Tween(GameObject go, float duration, Vector3 pos, UnityAction finished = null, Space cSpace = Space.World)
        {
            TweenPos cls = TweenMain.Tween<TweenPos>(go, duration, finished);
            cls.CSpace = cSpace;
            cls.from = cls.value;
            cls.to = pos;
            cls.Start();
            return cls;
        }

        /// <summary>
        /// Create a TweenPos Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <param name="cSpace">A optional Arugmeant to define the coordnaite space to work in</param>
        /// <returns>Return reference to the new TweenPos component</returns>
        public static TweenPos Tween(GameObject go, float duration, Vector3 fromVal, Vector3 toVal,
            UnityAction finished = null, Space cSpace = Space.World)
        {
            return Tween(go, duration, fromVal, toVal, Style.Once, Method.Linear, finished, cSpace);
        }

        /// <summary>
        /// Create a TweenPos Component and start a tween
        /// </summary>
        /// <param name="go">GameObject to apply tween too</param>
        /// <param name="duration">Duration of tween</param>
        /// <param name="fromVal">The starting value for the tween</param>
        /// <param name="toVal">The ending value for the tween</param>
        /// <param name="style">The style of tween (Once, Looped, PingPong)</param>
        /// <param name="method">The Interpolation method of the tween</param>
        /// <param name="finished">A optional Callback to fire when the tween is done</param>
        /// <param name="cSpace">A optional Arugmeant to define the coordnaite space to work in</param>
        /// <returns>Return reference to the new TweenPos component</returns>
        public static TweenPos Tween(GameObject go, float duration, Vector3 fromVal, Vector3 toVal,
            Style style, Method method, UnityAction finished = null, Space cSpace = Space.World)
        {
            TweenPos cls = TweenMain.Tween<TweenPos>(go, duration, style, method, finished);
            cls.CSpace = cSpace;
            cls.from = fromVal;
            cls.to = toVal;
            cls.Start();
            return cls;
        }
    }
}
