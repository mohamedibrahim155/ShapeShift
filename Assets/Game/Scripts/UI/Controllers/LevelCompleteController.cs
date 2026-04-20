using Scripts.Level;
using Scripts.Player;
using System;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Scripts.UI
{
    public class LevelCompleteController : IController
    {
        // Services Dependencies
        private IPlayerService m_PlayerService;
        private IUIService m_UIService;
        private ILevelService m_LevelService;


        // UI Window
        private LevelCompleteWindow m_Window;

        private EWindowID ID = EWindowID.LevelCompleted;

        public LevelCompleteController(IUIService uiService, IPlayerService playerService, ILevelService levelService)
        {
            m_PlayerService = playerService;
            m_UIService = uiService;
            m_LevelService = levelService;
        }

        public void Initialize()
        {
            m_Window = m_UIService.GetWindow(EWindowID.LevelCompleted) as LevelCompleteWindow;


            m_Window.OnNextLevelClicked     += HandleNextLevelClicked;  
            m_Window.OnRemoveAdsClicked     += HandleRemoveAdsClicked;
            m_LevelService.OnLevelCompleted += HandleOnLevelCompleted;

        }

   

        private void HandleOnLevelCompleted()
        {
            m_Window.UpdateLevelText(m_LevelService.GetCurrentLevel());
            m_Window.OpenLevelCompleteScreenWithDelay();
        }

        private void HandleRemoveAdsClicked()
        {
            Debug.Log("Remove ads Clicked");
        }

        private void HandleNextLevelClicked()
        {
            m_UIService.CloseWindow(EWindowID.LevelCompleted);
            SpawnNextLevel();
        }

        /// <summary>
        ///    Spawn next level by cleaning up the current level, 
        ///    resetting player state and opening the main menu to let
        ///    player start the next level when they are ready.
        /// </summary>
        private void SpawnNextLevel()
        {
            m_LevelService.Cleanup();
            m_LevelService.SpawnNextLevel();

            m_PlayerService.Reset();

            m_UIService.OpenWindow(EWindowID.MinMenu);
        }


        /// <summary>
        /// Clean up event subscriptions to avoid memory leaks
        /// </summary>
        public void Cleanup()
        {
            if (m_Window == null)
            {
                return;
            }
            m_Window.OnNextLevelClicked     -= HandleNextLevelClicked;  
            m_Window.OnRemoveAdsClicked     -= HandleRemoveAdsClicked;
            m_LevelService.OnLevelCompleted -= HandleOnLevelCompleted;
        }

   
    }
}
