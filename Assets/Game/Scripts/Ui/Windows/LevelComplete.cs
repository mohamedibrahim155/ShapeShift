using Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
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

        [Inject]
        private void Construct(IPlayerService playerService, IUiService uiService)
        {
            m_PlayerService = playerService;
            m_UIService = uiService;

            NextLevelButton.onClick.AddListener(OnNextLevelClicked);
            RemoveAdsButton.onClick.AddListener(OnRemoveAdsClicked);
        }


        private void OnNextLevelClicked()
        {
            Debug.Log("Next level Pressed");

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

        private void SpawnNextLevel()
        {
            m_PlayerService.StartGame();
        }

    }
}
