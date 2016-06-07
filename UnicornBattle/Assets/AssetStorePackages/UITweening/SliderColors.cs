using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Tweening/Slider Colors")]
    [RequireComponent(typeof(Slider))]
    public class SliderColors : MonoBehaviour
    {
        public Image target;
        public Color[] Colors = {Color.red, Color.yellow, Color.green};

        private Slider slider;

        private void Start()
        {
            slider = GetComponent<Slider>();
            if (target == null)
                target = slider.GetComponentInChildren<Image>();

            slider.onValueChanged.AddListener(OnValueChanged);
            OnValueChanged(slider.value);
        }

        private void OnValueChanged(float value)
        {
            float val = value * (Colors.Length - 1);
            int startIndex = Mathf.FloorToInt(val);
            Color c = Colors[0];
            if ((startIndex + 1) < Colors.Length)
                c = Color.Lerp(Colors[startIndex], Colors[startIndex + 1], val - startIndex);
            else if (startIndex < Colors.Length)
                c = Colors[startIndex];
            target.color = c;
        }
    }
}
