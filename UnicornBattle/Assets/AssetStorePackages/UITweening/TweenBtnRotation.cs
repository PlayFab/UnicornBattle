using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Tweening/Interaction/Button Rotation")]
    public class TweenBtnRotation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Transform target;
        public TweenMain.Method method = TweenMain.Method.Linear;
        public TweenMain.Style style = TweenMain.Style.Once;
        public Vector3 hover = Vector3.zero;
        public Vector3 pressed = new Vector3(0f, 0f, 180f);
        public float duration = 0.2f;

        private RectTransform rect;
        private Quaternion _Rot;
        private bool _Started = false;
        private bool hovered = false;

        void Start()
        {
            if (!_Started)
            {
                _Started = true;
                if (target == null) target = transform;
                rect = target.GetComponent<RectTransform>();
                _Rot = rect.rotation;
            }
        }

        void OnDisable()
        {
            if (_Started && target != null)
            {
                TweenRot tc = target.GetComponent<TweenRot>();

                if (tc != null)
                    tc.value = _Rot;
                tc.enabled = false;
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (enabled)
            {
                if (!_Started) Start();
                TweenRot.Tween(target.gameObject, duration, rect.rotation, _Rot * Quaternion.Euler(pressed), style, method);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (enabled)
            {
                if (!_Started) Start();
                if (hovered)
                    TweenRot.Tween(target.gameObject, duration, rect.rotation, _Rot * Quaternion.Euler(hover), style, method);
                else
                    TweenRot.Tween(target.gameObject, duration, rect.rotation, _Rot, TweenMain.Style.Once, method);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            if (enabled)
            {
                if (!_Started) Start();
                TweenRot.Tween(target.gameObject, duration, rect.rotation, _Rot * Quaternion.Euler(hover), style, method);
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
            if (enabled)
            {
                if (!_Started) Start();
                TweenRot.Tween(target.gameObject, duration, rect.rotation, _Rot, TweenMain.Style.Once, method);
            }
        }
    }
}
