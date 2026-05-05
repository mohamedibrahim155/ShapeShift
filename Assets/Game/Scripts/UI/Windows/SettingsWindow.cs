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
        [SerializeField] private Button adButton;

        public event Action OnFacebookShareButtonClicked = delegate { };
        public event Action OnAdButtonClicked = delegate { };
        public event Action OnCloseClicked = delegate { };

        public event Action OnMusicSliderClicked = delegate { };
        public event Action OnHapticSliderClicked = delegate { };

        public enum ESettingSliderType
        {
            MUSIC = 0,
            HAPTIC = 1,
        }

        private void Awake()
        {
            CloseButton.onClick.AddListener(() => OnCloseClicked.Invoke());
            facebookShareButton.onClick.AddListener(() => OnFacebookShareButtonClicked.Invoke());
            adButton.onClick.AddListener(() => OnAdButtonClicked.Invoke());

            MusicSlider.OnKnobButtonClicked  += HandleOnMusicPressed;
            HapticSlider.OnKnobButtonClicked += HandleOnHapticPressed;
        }

      

        public void UpdateSlider(ESettingSliderType sliderType, float value)
        {
            if (sliderType == ESettingSliderType.MUSIC)
            {
                MusicSlider.SetSliderValue(value);
            }
            else
                HapticSlider.SetSliderValue(value);
        }

        private void HandleOnMusicPressed()
        {
            OnMusicSliderClicked.Invoke();
        }

        private void HandleOnHapticPressed()
        {
            OnHapticSliderClicked.Invoke();
        }

        private void OnDestroy()
        {
            CloseButton.onClick.RemoveAllListeners();
            facebookShareButton.onClick.RemoveAllListeners();
            adButton.onClick.RemoveAllListeners();

            MusicSlider.OnKnobButtonClicked  -= HandleOnMusicPressed;
            HapticSlider.OnKnobButtonClicked -= HandleOnHapticPressed;

        }

        public void Reset()
        {
            ID = EWindowID.Settings;
        }
    }
}
