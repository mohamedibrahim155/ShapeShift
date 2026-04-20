using Scripts.Player;
using System;
using Unity.Services.Core;
using UnityEngine;
using Zenject;

namespace Scripts.UI
{
    public class MainMenuController : IController
    {
        private readonly IUIService m_UIService;
        private readonly IPlayerService m_PlayerService;

        private MainMenu m_MainMenuWindow;

        
        public MainMenuController(IUIService uIService, IPlayerService playerService)
        {
            m_UIService = uIService;
            m_PlayerService = playerService;
        }

        public void Initialize()
        {
            m_MainMenuWindow =  m_UIService.GetWindow(EWindowID.MinMenu) as MainMenu;

            m_MainMenuWindow.OnPlayClicked += HandlePlayClicked;
            m_MainMenuWindow.OnQuitClicked += HandleQuitClicked;
            m_MainMenuWindow.OnShopClicked += HandleShopClicked;
        }

        private void HandleShopClicked()
        {
            m_UIService.OpenWindow(EWindowID.Shop);
        }

        private void HandleQuitClicked()
        {
            Application.Quit();
        }

        private void HandlePlayClicked()
        {
            m_PlayerService.StartGame();
            m_UIService.OpenWindow(EWindowID.Gameplay);
            m_UIService.CloseWindow(EWindowID.MinMenu);
        }

        public void Cleanup()

        {

            if (m_MainMenuWindow == null)
            {
                return;
            }
            m_MainMenuWindow.OnPlayClicked -= HandlePlayClicked;
            m_MainMenuWindow.OnQuitClicked -= HandleQuitClicked;
            m_MainMenuWindow.OnShopClicked -= HandleShopClicked;
        }
    }
}
