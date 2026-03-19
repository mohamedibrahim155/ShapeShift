using Scripts.Player;
using Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.UI
{
    public class MainMenu : UIWindow
    {
        [SerializeField] private Button PlayButton;
        [SerializeField] private Button OptionButton;
        [SerializeField] private Button QuitButton;

        public event Action OnPlayButtonClicked = delegate { };

        private IPlayerService m_PlayerService;
        private IUIService m_UIService;

        [Inject]
        private void Construct(IPlayerService playerService, IUIService uiService)
        {
            m_PlayerService = playerService;
            m_UIService = uiService;

            PlayButton.onClick.AddListener(OnPlayClicked);
            QuitButton.onClick.AddListener(OnQuitClicked);
        }


        private void OnQuitClicked()
        {
            Debug.Log("Quit Button Pressed");

            Application.Quit();
        }

        private void OnPlayClicked()
        {
            OnPlayButtonClicked.Invoke();
            m_PlayerService.StartGame();
            m_UIService.OpenWindow(EWindowID.Gameplay);
            Close();
        }

    }
}
