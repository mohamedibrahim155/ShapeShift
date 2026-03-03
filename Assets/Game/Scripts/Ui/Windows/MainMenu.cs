using Scripts.Player;
using Scripts.UI;
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

        private IPlayerService m_PlayerService;
        private IUiService m_UIService;

        [Inject]
        private void Construct(IPlayerService playerService, IUiService uiService)
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
            Debug.Log("Play Button Pressed");
            m_PlayerService.StartGame();
        }

    }
}
