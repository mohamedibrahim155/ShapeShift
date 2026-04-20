using Scripts.Ads;
using Scripts.Level;
using Scripts.Player;
using System;
using UnityEngine;
using Zenject;

namespace Scripts.UI
{
    public class LevelFailedController : IController
    {

        // Services Dependencies
        private IUIService m_UIService;
        private IPlayerService m_PlayerService;
        private ILevelService m_LevelService;
        private IAdService m_adService;

        private LevelFailedWindow m_Window;


        public  LevelFailedController(IUIService uiservice, IPlayerService playerService, ILevelService levelService, IAdService adService )
        {
            m_UIService = uiservice;
            m_PlayerService = playerService;
            m_LevelService = levelService;
            m_adService = adService;
        }

        public void Initialize()
        {
            m_Window = m_UIService.GetWindow(EWindowID.LevelFailed) as LevelFailedWindow;

            m_Window.WatchAdButtonView.OnButtonClicked     += HandleWatchAdButtonClicked;
            m_Window.OnRetryButtonClicked                  += HandleRetryClicked;
            m_LevelService.OnLevelFailed                   += HandleLevelFailed;
        }


        private void HandleLevelFailed()
        {
            m_Window.UpdateLevelText(m_LevelService.GetCurrentLevel());
            m_Window.OpenLevelFailedWindowWithDelay();
        }


        private void HandleWatchAdButtonClicked()
        {
            m_Window.WatchAdButtonView.SetInteractable(false);

            m_adService.ShowAd(EAdType.Intersetial, OnAdComplete);
            void OnAdComplete(RewardedAdResult result)
            {
                Debug.Log("Watch ad result: " + result.ToString());
                m_Window.WatchAdButtonView.SetInteractable(true);

            }
        }

        private void ResetLevel()
        {
            int currentLevel = m_LevelService.GetCurrentLevel();

            m_LevelService.Cleanup();
            m_LevelService.SpawnLevel(currentLevel);

            m_PlayerService.Reset();


        }

        private void HandleRetryClicked()
        {
            m_UIService.CloseWindow(EWindowID.LevelFailed);
            ResetLevel();
            m_UIService.OpenWindow(EWindowID.MinMenu);

        }

        public void Cleanup()
        {
            if (m_Window == null) return;

            m_Window.WatchAdButtonView.OnButtonClicked -= HandleWatchAdButtonClicked;
            m_Window.OnRetryButtonClicked              -= HandleRetryClicked;
            m_LevelService.OnLevelFailed               -= HandleLevelFailed;
        }
    }
}
