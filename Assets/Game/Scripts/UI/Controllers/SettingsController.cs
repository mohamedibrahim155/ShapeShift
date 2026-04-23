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


        private  IHapticService m_HapticService;
        private SettingsWindow m_SettingWindow;
        private bool m_IsMusicEnabled  = true;
        public SettingsController(IUIService uIService, IHapticService hapticService )
        {
            m_UIService = uIService;
            m_HapticService = hapticService;
        }

    
        public void Initialize()
        {
            m_SettingWindow = m_UIService.GetWindow(EWindowID.Settings) as SettingsWindow;

            m_SettingWindow.OnCloseClicked                      += HandleCloseSettings;
            m_SettingWindow.OnFacebookShareButtonClicked        += HandleFacebookShareButtonClicked;
            m_SettingWindow.OnMusicSliderClicked                += HandleMusicButtonClicked;
            m_SettingWindow.OnHapticSliderClicked               += HandleHapticButtonClicked;


            m_HapticService.Config.OnHapticEnabledChanged       += SetHapticUI;
            SetHapticUI(m_HapticService.Config.isHapticEnabled);



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
