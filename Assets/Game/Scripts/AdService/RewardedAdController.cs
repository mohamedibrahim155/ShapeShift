using Scripts.UI;
using System;
using UnityEngine;
using Zenject;

namespace Scripts.Ads
{
    public class RewardedAdController
    {

        private readonly IUIService _uiService;
        private readonly IAdService _adService;
        private LevelFailedWindow _window;


        public RewardedAdController(IUIService uiService, IAdService adService)
        {
            _uiService = uiService;
            _adService = adService;
        }

        public void Initialize()
        {
            Debug.Log("Ontizualsed");
            _window = _uiService.GetWindow(EWindowID.LevelFailed) as LevelFailedWindow;

            _window.WatchAdButtonView.OnButtonClicked += HandleWatchAdButtonClicked;
        }

        private void HandleWatchAdButtonClicked()
        {
            _adService.ShowAd(EAdType.Rewarded, OnAdComplete);

            void OnAdComplete(RewardedAdResult result)
            {
                Debug.Log("Watch ad result: " + result.ToString());
            }
        }

        public void Dispose()
        {
            _window.WatchAdButtonView.OnButtonClicked -= HandleWatchAdButtonClicked;
        }


    }
}
