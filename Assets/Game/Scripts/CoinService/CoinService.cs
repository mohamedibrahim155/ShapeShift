using Scripts.UI.Coins;
using Scripts.Utilities.Pool;
using System;
using UnityEngine;
using Zenject;

namespace Scripts.UI.Coins
{
    public class CoinService : ICoinService
    {
        public int CurrentCoins { get; private set; }
        public event Action<int> OnCoinsChanged = delegate { };

        public CoinPool CoinPool { get; private set; }
        public CoinConfig CoinConfig { get; private set; }

        [Inject]
        public void Construct( CoinConfig coinConfig)
        {            
            CoinConfig = coinConfig;
        }

        public void AddCoins(int amount)
        {
            if (amount<=0)
            {
                return;
            }
            CurrentCoins += amount;
            OnCoinsChanged.Invoke(CurrentCoins);
        }

        public void RemoveCoins(int amount)
        {
            CurrentCoins -= amount;
            CurrentCoins =  Mathf.Max(CurrentCoins, 0);
            OnCoinsChanged.Invoke(CurrentCoins);
        }

        public void ResetCoins()
        {
            CurrentCoins = 0;
            OnCoinsChanged.Invoke(CurrentCoins);
        }
    }
}
