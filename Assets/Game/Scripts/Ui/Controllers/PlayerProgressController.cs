using Scripts.GameService;
using Scripts.Player;
using UnityEngine;
using Zenject;

namespace Scripts.UI
{
    public class PlayerProgressController
    {

        private  IGameLoopService m_GameLoopService;
        private  IPlayerService m_PlayerService;
        private  IUIService m_UIService;

        private GameWindow m_GameWindow;
        private MainMenu m_MainMenuWindow;

        private bool isRunning;

        [Inject]
        public void Construct( IGameLoopService gameLoopService, IPlayerService playerService, IUIService uIService)
        {
            m_GameLoopService = gameLoopService;
            m_PlayerService = playerService;
            m_UIService = uIService;

            m_GameWindow = m_UIService.GetWindow(EWindowID.Gameplay) as GameWindow;
            m_MainMenuWindow =  m_UIService.GetWindow(EWindowID.MinMenu) as MainMenu;


            m_MainMenuWindow.OnPlayButtonClicked += StartTracking;
            m_PlayerService.OnPlayerDied += OnDied;
            m_PlayerService.OnPlayerFinishedLevel += OnFinishedLevel;
        }

        public void StartTracking()
        {
            Debug.Log("OnPlayButtonClicked Clicked");

            if (isRunning  || m_GameWindow == null)
            {
                return;
            }

            isRunning =true;

            m_GameWindow.ResetProgress();
            m_GameLoopService.OnUpdateTick += UpdateProgress;

        }

        public void StopTracking()
        {
            if (!isRunning)
            {
                return;
            }

            isRunning = false;

            m_GameLoopService.OnUpdateTick -= UpdateProgress;
        }



        private void UpdateProgress()
        {

            Debug.Log("Updating progress");
            if(m_GameWindow ==  null)
                return;

            float totalDistance = m_PlayerService.GetTotalProgressedDistance();

            if (totalDistance <= 0)
            {
                m_GameWindow.ResetProgress();
                return;
            }


            float remainIngDistance = m_PlayerService.GetPlayerProgressedDistance();

            float progress =  1 - (remainIngDistance / totalDistance);
            
            m_GameWindow.SetProgress(progress);

        }

        private void OnDied()
        {
            m_GameWindow?.ResetProgress();
            StopTracking();
        }

        private void OnFinishedLevel()
        {
            m_GameWindow?.SetProgress(1);
            StopTracking();
        }

        public void Cleanup()
        {
            m_MainMenuWindow.OnPlayButtonClicked    -= StartTracking;
            m_PlayerService.OnPlayerDied            -= OnDied;
            m_PlayerService.OnPlayerFinishedLevel   -= OnFinishedLevel;
        }

    }
}