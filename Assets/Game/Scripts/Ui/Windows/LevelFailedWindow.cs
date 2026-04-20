using Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Scripts.Level;
using System.Collections;
using DG.Tweening;
using Scripts.Ads;
using System;
namespace Scripts.UI
{
    public class LevelFailedWindow : UIWindow
    {
        [SerializeField] private ButtonVisuals RetryButton;
        [SerializeField] private TextMeshProUGUI TotalCoinsTextField;
        [SerializeField] private GameObject LevelFailedBanner;
        [SerializeField] private TextMeshProUGUI LevelFailedTextField;
        [SerializeField] private TextMeshProUGUI LevelNumberTextField;
        [SerializeField] private RewardedAdButtonView WatchAdButton;

        public RewardedAdButtonView WatchAdButtonView => WatchAdButton;
        public Action OnRetryButtonClicked = delegate { };



        private void Awake()
        {
            RetryButton.Button.onClick.AddListener(() => OnRetryButtonClicked.Invoke());
        }

        public void OpenLevelFailedWindowWithDelay()
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

        private void AnimateBanner()
        {
            LevelFailedBanner.transform.DOScale(Vector3.one, 0.3f).OnComplete(() => AnimateLevelText()).SetEase(Ease.InOutCubic);
        }

        private void AnimateLevelText()
        {
            LevelFailedTextField.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutCubic).OnComplete(() => FadeRetryButton(1, 0.25f));
        }

        public void UpdateLevelText(int levelNumber)
        {
            LevelNumberTextField.text = $"Level {levelNumber}";
        }


        public void FadeLevelNumber(float value, float time)
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
            RetryButton.Button.onClick.RemoveAllListeners();
        }


    }
}
