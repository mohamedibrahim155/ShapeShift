using Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Scripts.Level;
using System.Collections;
using DG.Tweening;
using Scripts.Ads;
using System;
namespace Scripts.UI
{
    public class SettingsWindow : UIWindow
    {
        [SerializeField] private SliderView MusicSlider;
        [SerializeField] private SliderView HapticSlider;
        [SerializeField] private Button CloseButton;

        [SerializeField] private Button facebookShareButton;

        public event Action OnFacebookShareButtonClicked = delegate { };
        public event Action OnCloseClicked = delegate { };

        public event Action OnMusicSliderClicked = delegate { };
        public event Action OnHapticSliderClicked = delegate { };
        private void Awake()
        {
            CloseButton.onClick.AddListener(() => OnCloseClicked.Invoke());
            facebookShareButton.onClick.AddListener(() => OnFacebookShareButtonClicked.Invoke());

            MusicSlider.OnKnobButtonClicked += () => OnMusicSliderClicked.Invoke();
            HapticSlider.OnKnobButtonClicked += () => OnHapticSliderClicked.Invoke();
        }

        public void UpdateMusicSlider(float value)
        {
            MusicSlider.SetSliderValue(value);
        }

        public void UpdateHapticSlider(float value)
        {
            HapticSlider.SetSliderValue(value);
        }

        private void OnDestroy()
        {
            CloseButton.onClick.RemoveAllListeners();
            facebookShareButton.onClick.RemoveAllListeners();

            MusicSlider.OnKnobButtonClicked  -= () => OnMusicSliderClicked.Invoke();
            HapticSlider.OnKnobButtonClicked -= () => OnHapticSliderClicked.Invoke();

        }

        public void Reset()
        {
            ID = EWindowID.Settings;
        }
    }
}
