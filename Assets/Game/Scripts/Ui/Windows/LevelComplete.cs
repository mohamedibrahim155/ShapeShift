using Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Scripts.Level;
namespace Scripts.UI
{
    public class LevelComplete : UIWindow
    {
        [SerializeField] private Button NextLevelButton;
        [SerializeField] private Button RemoveAdsButton;
        [SerializeField] private TextMeshProUGUI LevelNumberTextField;
        [SerializeField] private TextMeshProUGUI TotalCoinsTextField;

        private IPlayerService m_PlayerService;
        private IUiService m_UIService;
        private ILevelService m_LevelService;

        [Inject]
        private void Construct(IPlayerService playerService, IUiService uiService, ILevelService levelService)
        {
            m_PlayerService = playerService;
            m_UIService = uiService;
            m_LevelService = levelService;

            NextLevelButton.onClick.AddListener(OnNextLevelClicked);
            RemoveAdsButton.onClick.AddListener(OnRemoveAdsClicked);
        }


        private void OnNextLevelClicked()
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

        public override void Open(float time)
        {
            int currentLevel = m_LevelService.GetCurrentLevel();
            UpdateLevelText(currentLevel);

            base.Open(time);
        }

        private void SpawnNextLevel()
        {
            int GetNextLevel = m_LevelService.GetCurrentLevel();

            m_LevelService.UpdateLevel(GetNextLevel + 1);

            m_PlayerService.Reset();
        }

    }
}
