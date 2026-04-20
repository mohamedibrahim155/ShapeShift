using Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Scripts.Level;
using System.Collections;
using System;
namespace Scripts.UI
{
    public class LevelCompleteWindow : UIWindow
    {
        [SerializeField] private ButtonVisuals NextLevelButton;
        [SerializeField] private ButtonVisuals RemoveAdsButton;
        [SerializeField] private TextMeshProUGUI LevelNumberTextField;
        [SerializeField] private TextMeshProUGUI TotalCoinsTextField;

        // Services
        private IPlayerService m_PlayerService;
        private IUIService m_UIService;
        private ILevelService m_LevelService;

        // Events
        public Action OnNextLevelClicked = delegate { };
        public Action OnRemoveAdsClicked = delegate { };


        private void Awake()
        {
            NextLevelButton.Button.onClick.AddListener(() => OnNextLevelClicked.Invoke());
            RemoveAdsButton.Button.onClick.AddListener(() => OnRemoveAdsClicked.Invoke());
        }
        private IEnumerator DelayOpenCallback(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Open();
        }
        public void OpenLevelCompleteScreenWithDelay()
        {
            StartCoroutine(DelayOpenCallback(1));
        }

        public void UpdateLevelText(int levelNumber)
        {
            LevelNumberTextField.text = $"Level {levelNumber}";
        }


        private void OnDestroy()
        {
            NextLevelButton.Button.onClick.RemoveAllListeners();
            RemoveAdsButton.Button.onClick.RemoveAllListeners();
        }

    }
}
