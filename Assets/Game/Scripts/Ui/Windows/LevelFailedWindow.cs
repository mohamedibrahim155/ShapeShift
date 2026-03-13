using Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Scripts.Level;
namespace Scripts.UI
{
    public class LevelFailedWindow : UIWindow
    {
        [SerializeField] private Button RetryButton;
        [SerializeField] private TextMeshProUGUI TotalCoinsTextField;

        private IPlayerService m_PlayerService;
        private IUIService m_UIService;
        private ILevelService m_LevelService;

        [Inject]
        private void Construct(IPlayerService playerService, IUIService uiService, ILevelService levelService)
        {
            m_PlayerService = playerService;
            m_UIService = uiService;
            m_LevelService = levelService;

            RetryButton.onClick.AddListener(RetryButtonClicked);

            m_LevelService.OnLevelFailed += OpenLevelFailedWindow;
        }

        private void OpenLevelFailedWindow()
        {
            Debug.Log("Level Failed Window Opened");
            Open();
        }

        private void RetryButtonClicked()
        {
            Debug.Log("Retry  Pressed");

            Close();
            RestartLevel();
        }



        private void RestartLevel()
        {

            int currentLevel = m_LevelService.GetCurrentLevel();

            m_LevelService.Cleanup();
            m_LevelService.SpawnLevel(currentLevel);

            m_PlayerService.Reset();

            m_UIService.OpenWindow(EWindowID.MinMenu);
        }

        private void UnsubscribeEvents()
        {
            m_LevelService.OnLevelFailed -= OpenLevelFailedWindow;

            RetryButton.onClick.RemoveListener(RetryButtonClicked);
        }
   

    }
}
