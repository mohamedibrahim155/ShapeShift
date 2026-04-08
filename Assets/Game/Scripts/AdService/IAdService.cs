
using System;
using UnityEngine;

namespace Scripts.Ads
{
    public enum RewardedAdResult
    {
        Completed,
        Skipped,
        Failed,
        NotReady
    }
    public enum EAdType
    {
        Rewarded,
        Intersetial,
        Banner
    }

    public interface IAdService 
    {
        AdsConfig AdsConfig { get; }
        bool IsAdsInitialized { get; }
        event Action OnAdInitialization; // Event to notify when an ad has been completed
        event Action<string> OnAdInitalizationFailed; // Event to notify when an ad has been completed
        event Action<EAdType> OnAdLoaded;
        event Action<EAdType,string> OnAdLoadedFailed;
        event Action<EAdType,string> OnAdShowFailed;
        void Intialize();

        void LoadAd(EAdType adType);
        void ShowAd(EAdType adType, Action<RewardedAdResult> onAdComplete);
    }
}
