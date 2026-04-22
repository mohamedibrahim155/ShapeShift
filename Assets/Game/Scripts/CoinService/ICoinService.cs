using Scripts.Utilities.Pool;
using System;
using UnityEngine;

namespace Scripts.UI.Coins
{
    public interface ICoinService
    {
        int CurrentCoins { get; }
        CoinConfig CoinConfig { get; }
        CoinPool CoinPool { get; }

        event Action<int> OnCoinsChanged;

        void AddCoins(int amount);
        void RemoveCoins(int amount);

        void ResetCoins();
    }
}
