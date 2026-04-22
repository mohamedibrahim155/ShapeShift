using Scripts.UI.Coins;
using System.Drawing;
using UnityEngine;

namespace Scripts.Utilities.Pool
{
    public class CoinPool : GenericPool<CoinView>
    {
        private Transform parent;
        public CoinPool(CoinView prefab, int initialPoolSize, Transform parent) : base (prefab, initialPoolSize)
        {
            this.parent = parent;
        }

        protected override CoinView CreateInstance()
        {
            CoinView instance = base.CreateInstance();
            instance.transform.SetParent(parent);
            instance.Hide();
            return instance;
        }

        public CoinView GetCoin()
        {
            CoinView coin = Get();
            coin.Show();
            return coin;
        }

        public override void ReturnToPool(CoinView coinInstance)
        {
            coinInstance.Hide();
            base.ReturnToPool(coinInstance);
        }

        public class CoinBuilder
        {
            private CoinView prefab;
            private Transform parent;
            private int initialPoolSize = 10;
            public CoinBuilder SetPrefab(CoinView prefab)
            {
                this.prefab = prefab;
                return this;
            }
            public CoinBuilder SetInitialPoolSize(int size)
            {
                this.initialPoolSize = size;
                return this;
            }

            public CoinBuilder SetParent (Transform parent)
            {
                this.parent = parent;
                return this;
            }
            public CoinPool Build()
            {
                if (prefab == null)
                {
                    throw new System.Exception("Prefab must be set before building the CoinPool.");
                }
                var coinPool = new CoinPool(prefab, initialPoolSize, parent);
                coinPool.Initialize();
                return coinPool;
            }
        }
        

    }
}
