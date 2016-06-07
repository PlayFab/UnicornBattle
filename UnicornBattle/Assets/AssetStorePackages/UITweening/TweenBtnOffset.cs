using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Tweening/Interaction/Button Offset")]
    public class TweenBtnOffset : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Transform target;
        public TweenMain.Method method = TweenMain.Method.Linear;
        public TweenMain.Style style = TweenMain.Style.Once;
        public Vector3 hover = Vector3.zero;
        public Vector3 pressed = new Vector3(2f, -2f);
        public float duration = 0.2f;

        private RectTransform rect;
        private Vector3 _Pos;
        private bool _Started = false;
        private bool hovered = false;

        void Start()
        {
            if (!_Started)
            {
                _Started = true;
                if (target == null) target = transform;
                rect = target.GetComponent<RectTransform>();
                _Pos = rect.position;
            }
        }

        void OnDisable()
        {
            if (_Started && target != null)
            {
                TweenPos tc = target.GetComponent<TweenPos>();

                if (tc != null)
                    tc.value = _Pos;
                tc.enabled = false;
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (enabled)
            {
                if (!_Started) Start();
                TweenPos.Tween(target.gameObject, duration, rect.position, _Pos + pressed, style, method);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (enabled)
            {
                if (!_Started) Start();
                if (hovered)
                    TweenPos.Tween(target.gameObject, duration, rect.position, _Pos + hover, style, method);
                else
                    TweenPos.Tween(target.gameObject, duration, rect.position, _Pos, TweenMain.Style.Once, method);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            if (enabled)
            {
                if (!_Started) Start();
                TweenPos.Tween(target.gameObject, duration, rect.position, _Pos + hover, style, method);
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
            if (enabled)
            {
                if (!_Started) Start();
                TweenPos.Tween(target.gameObject, duration, rect.position, _Pos, TweenMain.Style.Once, method);
            }
        }
    }
}
