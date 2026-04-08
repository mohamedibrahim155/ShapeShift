using UnityEngine;

namespace Scripts.Ads
{
    [CreateAssetMenu(fileName = "AdsConfig", menuName = "Scriptable Objects/Ads/AdsConfig")]
    public class AdsConfig : ScriptableObject
    {
        [Header("Unity Ads App Ids")]
        [SerializeField] private string m_androidAdUnityId;
        [SerializeField] private string m_IosAdUnityId;

        [SerializeField] private string m_rewardedAdUnitId = "Rewarded_Android"; // Default value for testing, can be overridden in the Inspector
        [SerializeField] private string rewardedIosAdUnitId = "Rewarded_iOS";
        [SerializeField] private string m_intersitial_AndroidPlacementId = "Interstitial_Android";
        [SerializeField] private string m_intersitial_IOSPlacementId = "Interstitial_iOS";

        [Header("Flags")]
        [SerializeField] private bool m_TestMode =true; // Set to false for production builds
        [SerializeField] private bool m_AdsEnabled = true; // Flag to enable or disable ads globally

        public string AndroidAdUnityId => m_androidAdUnityId;
        public string IosAdUnityId => m_IosAdUnityId;
        public string Android_RewardedID => m_rewardedAdUnitId;
        public string IOS_RewardedID => rewardedIosAdUnitId;
        public string Android_IntersitialAdID => m_intersitial_AndroidPlacementId;
        public string IOS_IntersitalAdID => m_intersitial_IOSPlacementId;
        public bool TestMode => m_TestMode;
        public bool AdsEnabled => m_AdsEnabled;
    }
}
