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
        private readonly IPlayerService m_PlayerService;

        private SettingsWindow m_SettingWindow;

        private bool m_IsMusicEnabled  = true;
        private bool m_IsHapticEnabled = true;
        public SettingsController(IUIService uIService)
        {
            m_UIService = uIService;
        }
        public void Initialize()
        {
            m_SettingWindow = m_UIService.GetWindow(EWindowID.Settings) as SettingsWindow;

            m_SettingWindow.OnCloseClicked               += HandleCloseSettings;
            m_SettingWindow.OnFacebookShareButtonClicked += HandleFacebookShareButtonClicked;
            m_SettingWindow.OnMusicSliderClicked         += HandleMusicButtonClicked;
            m_SettingWindow.OnHapticSliderClicked        += HandleHapticButtonClicked;


        }
        public void Cleanup()
        {
            if (m_SettingWindow == null) return;

            m_SettingWindow.OnCloseClicked               -= HandleCloseSettings;
            m_SettingWindow.OnFacebookShareButtonClicked -= HandleFacebookShareButtonClicked;
            m_SettingWindow.OnMusicSliderClicked         -= HandleMusicButtonClicked;
            m_SettingWindow.OnHapticSliderClicked        -= HandleHapticButtonClicked;
        }


        /// Toggle music on and off by muting the audio listener. This is a simple implementation, you can expand this by adding a music manager that handles music and sound 
        private void HandleHapticButtonClicked()
        {
            m_IsHapticEnabled = !m_IsHapticEnabled;
            m_SettingWindow.UpdateHapticSlider(m_IsHapticEnabled ? 1f : 0f);
        }

        /// <summary>
        /// Toggle handle haptics on or off
        /// </summary>
        private void HandleMusicButtonClicked()
        {
            m_IsMusicEnabled = !m_IsMusicEnabled;
            m_SettingWindow.UpdateMusicSlider((m_IsMusicEnabled) ? 1f : 0f);
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
