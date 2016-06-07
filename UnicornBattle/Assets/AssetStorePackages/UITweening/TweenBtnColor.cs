using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Tweening/Interaction/Button Color")]
    public class TweenBtnColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Transform target;
        public TweenMain.Method method = TweenMain.Method.Linear;
        public TweenMain.Style style = TweenMain.Style.Once;
        public Color hover = Color.black;
        public Color pressed = Color.black;
        public float duration = 0.2f;

        private Graphic gfx;
        private Color _Col;
        private bool _Started = false;
        private bool hovered = false;

        void Start()
        {
            if (!_Started)
            {
                _Started = true;
                if (target == null) target = transform;
                gfx = target.GetComponent<Graphic>();
                _Col = gfx.color;
            }
        }

        void OnDisable()
        {
            if (_Started && target != null)
            {
                TweenColor tc = target.GetComponent<TweenColor>();
                if (tc != null)
                    tc.value = _Col;
                tc.enabled = false;
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (enabled)
            {
                if (!_Started) Start();
                TweenColor.Tween(target.gameObject, duration, gfx.color, pressed, style, method);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (enabled)
            {
                if (!_Started) Start();
                if (hovered)
                    TweenColor.Tween(target.gameObject, duration, gfx.color, hover, style, method);
                else
                    TweenColor.Tween(target.gameObject, duration, gfx.color, _Col, TweenMain.Style.Once, method);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            if (enabled)
            {
                if (!_Started) Start();
                TweenColor.Tween(target.gameObject, duration, gfx.color, hover, style, method);
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
            if (enabled)
            {
                if (!_Started) Start();
                TweenColor.Tween(target.gameObject, duration, gfx.color, _Col, TweenMain.Style.Once, method);
            }
        }
    }
}
