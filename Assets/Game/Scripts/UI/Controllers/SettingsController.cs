using Scripts.Audio;
using Scripts.Haptics;
using Scripts.Player;
using System;
using Unity.Services.Core;
using UnityEngine;
using Zenject;

namespace Scripts.UI
{
    public class SettingsController : IController
    {
        private readonly IUIService m_UIService;
        private readonly IHapticService m_HapticService;
        private readonly IAudioService m_AudioService;


        private SettingsWindow m_SettingWindow;
        private bool m_IsMusicEnabled  = true;
        public SettingsController(IUIService uIService, IHapticService hapticService, IAudioService audioService )
        {
            m_UIService = uIService;
            m_HapticService = hapticService;
            m_AudioService = audioService;
        }

    
        public void Initialize()
        {
            m_SettingWindow = m_UIService.GetWindow(EWindowID.Settings) as SettingsWindow;

            m_SettingWindow.OnCloseClicked                      += HandleCloseSettings;
            m_SettingWindow.OnFacebookShareButtonClicked        += HandleFacebookShareButtonClicked;
            m_SettingWindow.OnMusicSliderClicked                += HandleMusicButtonClicked;
            m_SettingWindow.OnHapticSliderClicked               += HandleHapticButtonClicked;


            m_HapticService.Config.OnHapticEnabledChanged       += SetHapticUI;
            m_AudioService.Config.OnMusicEnabledChanged         += SetMusicUI;

            
            SetHapticUI(m_HapticService.Config.isHapticEnabled);
            SetMusicUI(m_AudioService.Config.IsMusicEnabled);



        }

        private void SetMusicUI(bool isEnabled)
        {
            m_SettingWindow.UpdateMusicSlider(isEnabled ? 1f : 0f);
        }

        private void SetHapticUI(bool isEnabled)
        {
            m_SettingWindow.UpdateHapticSlider(isEnabled ? 1f : 0f);
        }

        public void Cleanup()
        {
            if (m_SettingWindow == null) return;

            m_SettingWindow.OnCloseClicked               -= HandleCloseSettings;
            m_SettingWindow.OnFacebookShareButtonClicked -= HandleFacebookShareButtonClicked;
            m_SettingWindow.OnMusicSliderClicked         -= HandleMusicButtonClicked;
            m_SettingWindow.OnHapticSliderClicked        -= HandleHapticButtonClicked;

            m_HapticService.Config.OnHapticEnabledChanged -= SetHapticUI;
            m_AudioService.Config.OnMusicEnabledChanged   -= SetMusicUI;
        }


        /// Toggle music on and off by muting the audio listener. This is a simple implementation, you can expand this by adding a music manager that handles music and sound 
        private void HandleHapticButtonClicked()
        {
            if (m_HapticService.Config == null) return;

            m_HapticService.SetHapticEnabled(!m_HapticService.Config.isHapticEnabled);
        }

        /// <summary>
        /// Toggle handle haptics on or off
        /// </summary>
        private void HandleMusicButtonClicked()
        {
            if (m_AudioService.Config == null) return;

            m_AudioService.SetMusicEnabled(!m_AudioService.Config.IsMusicEnabled);
        }

        private void HandleFacebookShareButtonClicked()
        {
            Debug.Log("Share facebook market here");
        }

        /// <summary>
        /// Closes the Settings window
        /// </summary>
        private void HandleCloseSettings()
        {
           m_UIService.CloseWindow(EWindowID.Settings);
        }

    
    }
  
}
