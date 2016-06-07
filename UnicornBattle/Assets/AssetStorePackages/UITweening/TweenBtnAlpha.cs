using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Tweening/Interaction/Button Alpha")]
    public class TweenBtnAlpha : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Transform target;
        public TweenMain.Method method = TweenMain.Method.Linear;
        public TweenMain.Style style = TweenMain.Style.Once;
        [Range(0f, 1f)]
        public float
            hover = 1f,
            pressed = 1f;
        public float duration = 0.2f;

        private Graphic gfx;
        private float _Alpha;
        private bool _Started = false;
        private bool hovered = false;

        void Start()
        {
            if (!_Started)
            {
                _Started = true;
                if (target == null) target = transform;
                gfx = target.GetComponent<Graphic>();
                _Alpha = gfx.color.a;
            }
        }

        void OnDisable()
        {
            if (_Started && target != null)
            {
                TweenAlpha tc = target.GetComponent<TweenAlpha>();
                if (tc != null)
                    tc.value = _Alpha;
                tc.enabled = false;
            }
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (enabled)
            {
                if (!_Started) Start();
                TweenAlpha.Tween(target.gameObject, duration, gfx.color.a, pressed, style, method);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (enabled)
            {
                if (!_Started) Start();
                if (hovered)
                    TweenAlpha.Tween(target.gameObject, duration, gfx.color.a, hover, style, method);
                else
                    TweenAlpha.Tween(target.gameObject, gfx.color.a, duration, _Alpha, TweenMain.Style.Once, method);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            if (enabled)
            {
                if (!_Started) Start();
                TweenAlpha.Tween(target.gameObject, duration, gfx.color.a, hover, style, method);
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
            if (enabled)
            {
                if (!_Started) Start();
                TweenAlpha.Tween(target.gameObject, duration, gfx.color.a, _Alpha, TweenMain.Style.Once, method);
            }
        }
    }
}
