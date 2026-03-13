using Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Scripts.Level;
namespace Scripts.UI
{
    public class LevelCompleteWindow : UIWindow
    {
        [SerializeField] private Button NextLevelButton;
        [SerializeField] private Button RemoveAdsButton;
        [SerializeField] private TextMeshProUGUI LevelNumberTextField;
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

        

            NextLevelButton.onClick.AddListener(FailedLevelButtonClicked);
            RemoveAdsButton.onClick.AddListener(OnRemoveAdsClicked);

            m_LevelService.OnLevelCompleted += OpenLevelCompleteScreen;
        }

        private void OpenLevelCompleteScreen()
        {
            Debug.Log("Opend the level complete screen");
            Open();
        }

        private void OnDisable()
        {
           // m_LevelService.OnLevelCompleted -= OpenLevelCompleteScreen;
        }

        private void FailedLevelButtonClicked()
        {
            Debug.Log("Next level Pressed");

            Close();
            SpawnNextLevel();
        }

        private void OnRemoveAdsClicked()
        {
            Debug.Log("Remove ads Pressed");
            //m_PlayerService.StartGame();
        }

        public void UpdateLevelText(int levelNumber)
        {
            LevelNumberTextField.text = $"Level {levelNumber}";
        }

        public override void Open(float time = 0.5f)
        {
            int currentLevel = m_LevelService.GetCurrentLevel();
            UpdateLevelText(currentLevel);

            base.Open(time);
        }

        //cleanup current level, then spawn next level, reset player and open min menu window to start next level when player clicks start button there.
        private void SpawnNextLevel()
        {
            m_LevelService.Cleanup();
            m_LevelService.SpawnNextLevel();

            m_PlayerService.Reset();

            m_UIService.OpenWindow(EWindowID.MinMenu);

        }

        private void OnDestroy()
        {
            UnsubcribeEvents();
        }
        private void UnsubcribeEvents()
        {
            m_LevelService.OnLevelCompleted -= OpenLevelCompleteScreen;

            NextLevelButton.onClick.RemoveAllListeners();
            RemoveAdsButton.onClick.RemoveAllListeners();
        }

    }
}
