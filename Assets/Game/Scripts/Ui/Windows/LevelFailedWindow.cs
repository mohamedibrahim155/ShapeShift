using Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Scripts.Level;
using System.Collections;
using DG.Tweening;
namespace Scripts.UI
{
    public class LevelFailedWindow : UIWindow
    {
        [SerializeField] private ButtonVisuals RetryButton;
        [SerializeField] private TextMeshProUGUI TotalCoinsTextField;
        [SerializeField] private GameObject LevelFailedBanner;
        [SerializeField] private TextMeshProUGUI LevelFailedTextField;
        [SerializeField] private TextMeshProUGUI LevelNumberTextField;

        private IPlayerService m_PlayerService;
        private IUIService m_UIService;
        private ILevelService m_LevelService;

        [Inject]
        private void Construct(IPlayerService playerService, IUIService uiService, ILevelService levelService)
        {
            m_PlayerService = playerService;
            m_UIService = uiService;
            m_LevelService = levelService;

            RetryButton.Button.onClick.AddListener(RetryButtonClicked);

            m_LevelService.OnLevelFailed += OpenLevelFailedWindow;
        }

        public void OpenLevelFailedWindow()
        {
            StartCoroutine(OpenWindowWithDelay(0.25f));
        }

        private IEnumerator OpenWindowWithDelay(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Open();
      
        }

        public override void Open(float time = 0.5F)
        {
            ResetWindow();
            base.Open(time);
            AnimateBanner();
            FadeLevelNumber(1, 0.25f);
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

            RetryButton.Button.onClick.RemoveListener(RetryButtonClicked);
        }

        private void AnimateBanner()
        {
            LevelFailedBanner.transform.DOScale(Vector3.one, 0.3f).OnComplete(() => AnimateLevelText()).SetEase(Ease.InOutCubic);
        }

        private void AnimateLevelText()
        {
            LevelFailedTextField.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutCubic).OnComplete(() => FadeRetryButton(1, 0.25f));
        }


        private void FadeLevelNumber(float value, float time)
        {
            LevelFailedTextField.DOFade(value, time);
        }

        private void FadeRetryButton(float value, float time)
        {
            RetryButton.Button.image.DOFade(value, time);
            RetryButton.ButtonTextField.DOFade(value, time);
        }

        private void ResetWindow()
        {
            LevelFailedBanner.transform.localScale = Vector3.zero;
            LevelFailedTextField.transform.localScale = Vector3.zero;
            FadeLevelNumber(0, 0);
            FadeRetryButton(0, 0);
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }


    }
}
