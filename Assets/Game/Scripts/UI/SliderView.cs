using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class SliderView : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image foregroundImage;
        [SerializeField] private RectTransform knob;
        [SerializeField] private Button KnobButton;

        [SerializeField, Range(0, 1)] private float sliderValue = 0f;

        private const float knobMoveWidth = 200f;
        public Action<float> OnValueChanged = delegate { };
        public Action OnKnobButtonClicked = delegate { };

        private void Awake()
        {
            KnobButton.onClick.AddListener(() => OnKnobButtonClicked.Invoke());
            ApplyVisuals(sliderValue);
        }

        public void SetSliderValue(float value)
        {
            sliderValue = Mathf.FloorToInt(value);
            ApplyVisuals(sliderValue);
            OnValueChanged?.Invoke(sliderValue);
        }


        private void ApplyVisuals(float value)
        {
            if (foregroundImage != null)
                foregroundImage.fillAmount = value;

            if (knob != null)
                knob.anchoredPosition = new Vector2(value * knobMoveWidth, knob.anchoredPosition.y);
        }

        private void OnValidate()
        {
            sliderValue = Mathf.FloorToInt(sliderValue);
            ApplyVisuals(sliderValue);
        }

        private void OnDestroy()
        {
            KnobButton.onClick.RemoveAllListeners();
        }


    }
}
