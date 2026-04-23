using UnityEngine;

namespace Scripts.UI.Coins
{
    [CreateAssetMenu(fileName = "CoinConfig", menuName = "Scriptable Objects/Coins/CoinConfig")]
    public class CoinConfig : ScriptableObject
    {
        public CoinView coinViewPrefab;
        public int CoinPoolSize = 20;

        [Header("Coin Animation Settings")]
       public float spawnRadius = 10f;
       public float spawnDuration = 1f;
       public float moveDuration = 0.5f;
       public float delayStep = 0.3f;
       public float globalDelay = 1f;

       public uint MinCoinReward = 10;
       public uint MaxCoinReward = 20;

        public int GetRandomCoinReward()
        {
            uint min = (uint)Mathf.Min(MinCoinReward, MaxCoinReward);
            uint max = (uint)Mathf.Max(MinCoinReward, MaxCoinReward);

            return (int)Random.Range(min, max);
        }
    }
}
