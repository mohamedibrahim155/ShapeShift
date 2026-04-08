using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Advertisements;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Advertisements;
using Zenject; // Make sure to include the Unity Ads namespace
namespace Scripts.Ads
{
    public class AdService : IAdService, IUnityAdsInitializationListener,
        IUnityAdsLoadListener,
        IUnityAdsShowListener
    {
        private string _gameId;
        private string _rewardedAdUnitId;
        private string _intersetialAdID;

        public AdsConfig AdsConfig { get; private set; }

        public bool IsAdsInitialized => Advertisement.isInitialized;


        private Action<RewardedAdResult> OnAdCompleted;

        public event Action OnAdInitialization = delegate { };
        public event Action<string> OnAdInitalizationFailed = delegate { };
        public event Action<EAdType> OnAdLoaded = delegate { };
        public event Action<EAdType, string> OnAdLoadedFailed = delegate { };
        public event Action<EAdType, string> OnAdShowFailed = delegate { };

        [Inject]
        public void Construct(AdsConfig adsConfig)
        {
            AdsConfig = adsConfig;
            ResolveGameID();

            Intialize();
        }

        public void Intialize()
        {
            if (!AdsConfig.AdsEnabled || IsAdsInitialized) return;

            if (string.IsNullOrEmpty(_gameId))
            {
                OnAdInitalizationFailed.Invoke("Missing Game ID in AdConfig.");
                return;
            }
            Debug.Log("Ads initalized: " + Advertisement.isInitialized);
            Advertisement.Initialize(_gameId, AdsConfig.TestMode, this);
        }


        private void ResolveGameID()
        {
#if UNITY_IOS
            _gameId           = AdsConfig.IosAdUnityId;
            _rewardedAdUnitId = AdsConfig.IOS_RewardedID;
            _intersetialAdID  = AdsConfig.IOS_IntersitalAdID;
#else
            _gameId = AdsConfig.AndroidAdUnityId;
            _rewardedAdUnitId = AdsConfig.Android_RewardedID;
            _intersetialAdID = AdsConfig.Android_IntersitialAdID;

#endif
        }


        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");

            OnAdInitialization.Invoke();

            LoadAd(EAdType.Rewarded);
            LoadAd(EAdType.Intersetial);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            string errorMessage = $"Unity Ads Initialization Failed: {error.ToString()} - {message}";
            Debug.LogError(errorMessage);

            OnAdInitalizationFailed.Invoke(errorMessage);
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            EAdType adType = GetAdType(placementId);
            OnAdLoaded.Invoke(adType);
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            EAdType adType = GetAdType(placementId);
            string errorMessage = $"Failed to load ad: {placementId} - {error.ToString()} - {message}";
            Debug.LogError(errorMessage);

            OnAdLoadedFailed.Invoke(adType, errorMessage);
            LoadAd(adType);
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            EAdType adType = GetAdType(placementId);
            string errorMessage = $"Failed to show ad: {placementId} - {error.ToString()} - {message}";
            Debug.LogError(errorMessage);

            OnAdShowFailed.Invoke(adType, message);
            LoadAd(adType);
        }

        public void OnUnityAdsShowStart(string placementId)
        {
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log($"AdsShow Click: {placementId}");

        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log($"OnUnityAdsShowComplete: " + placementId + " - " + showCompletionState.ToString());

            EAdType nextAdTypToload = GetAdType(placementId);
            RewardedAdResult completionState = MapResult(showCompletionState);

            OnAdCompleted?.Invoke(completionState);
            OnAdCompleted = null;



            LoadAd(nextAdTypToload);

        }

        public void LoadAd(EAdType adType)
        {
            if (!IsAdReady()) return;

            Advertisement.Load(GetAdTypeID(adType), this);
        }
        public void ShowAd(EAdType adType, Action<RewardedAdResult> onAdComplete)
        {
            if (!IsAdReady())
            {
                onAdComplete?.Invoke(RewardedAdResult.NotReady);
                return;
            }

            OnAdCompleted = onAdComplete;
            Advertisement.Show(GetAdTypeID(adType), this);
        }

        private string GetAdTypeID(EAdType adType)
        {
            return (adType) switch
            {
                EAdType.Rewarded => _rewardedAdUnitId,
                EAdType.Intersetial => _intersetialAdID,
                _ => _intersetialAdID
            };
        }

        private EAdType GetAdType(string AdID)
        {
            if (string.Equals(AdID, _rewardedAdUnitId)) return EAdType.Rewarded;
            if (string.Equals(AdID, _intersetialAdID)) return EAdType.Intersetial;

            return EAdType.Intersetial;
        }

        private static RewardedAdResult MapResult(UnityAdsShowCompletionState showCompletionState)
        {
            return showCompletionState switch
            {
                UnityAdsShowCompletionState.COMPLETED => RewardedAdResult.Completed,
                UnityAdsShowCompletionState.SKIPPED => RewardedAdResult.Skipped,
                UnityAdsShowCompletionState.UNKNOWN => RewardedAdResult.Failed,
                _ => RewardedAdResult.Failed
            };
        }

        private bool IsAdReady()
        {
            bool adReady = IsAdsInitialized && AdsConfig.AdsEnabled;
            return adReady;

        }
    }
}
