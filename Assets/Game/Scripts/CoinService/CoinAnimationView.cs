using DG.Tweening;
using Scripts.Utilities.Pool;
using UnityEngine;
using Zenject;

namespace Scripts.UI.Coins
{
    public class CoinAnimationView : MonoBehaviour
    {
        private Transform coinSourceTransform;
        [Inject] private CoinConfig coinConfig;

        private CoinPool coinPool;
        private void Awake()
        {
            if (coinConfig == null)
            {
                Debug.LogError("Missing Coin config");
                return;
            }

            coinSourceTransform = new GameObject("Coins Holder").transform;
            coinSourceTransform.SetParent(transform);
            InitalizePool();
        }

        private void InitalizePool()
        {

            coinPool = new CoinPool.CoinBuilder()
                .SetPrefab(coinConfig.coinViewPrefab)
                .SetInitialPoolSize(coinConfig.CoinPoolSize)
                .SetParent(coinSourceTransform)
                .Build();
        }

        private void OnDestroy()
        {
            if (coinPool == null) return;

            coinPool.Cleanup();
        }



        public Sequence AddCoinAnimate(RectTransform source, RectTransform target,  System.Action<int> OnCoinUpdated,long startAmount, long addAmount)
        {
            if (coinPool == null || coinConfig == null || source == null || target == null)
                return null;

            int coinAmount = Mathf.Clamp((int)addAmount, 2, 12);
            long end = startAmount + addAmount;


            var master = DOTween.Sequence()
             .SetAutoKill(true)
             .SetLink(gameObject, LinkBehaviour.KillOnDestroy);


            Vector3 originialScale = target.localScale;

            for (int i = 0; i < coinAmount; i++)
            {
                CoinView coinView = coinPool.GetCoin();

              
                RectTransform coinRect = coinView.GetCoinIconRectTransform();
                coinRect.position = source.position;
                coinView.transform.SetParent(target);




                float t = i / (float)(coinAmount - 1);


                Vector2 burstOffset = UnityEngine.Random.insideUnitCircle * coinConfig.spawnRadius;

                //burst
                var burstSequence = coinRect.DOMove(source.position + (Vector3)burstOffset, coinConfig.spawnDuration).
                    SetEase(Ease.InOutSine).
                    SetDelay(t * coinConfig.delayStep).
                    SetAutoKill(true).
                    SetLink(coinView.gameObject, LinkBehaviour.KillOnDisable | LinkBehaviour.KillOnDestroy);

                //Move animation
                var moveSequence = coinRect.DOMove(target.position, coinConfig.moveDuration).
                    SetEase(Ease.InBack).
                    SetAutoKill(true).
                    SetLink(coinView.gameObject, LinkBehaviour.KillOnDisable | LinkBehaviour.KillOnDestroy).
                    OnComplete(() =>
                    {
                        long stepValue = (long)Mathf.Ceil(Mathf.Lerp(startAmount, end, t));

                        OnCoinUpdated?.Invoke((int)stepValue);
                        // totalText.text = stepValue.ToString();
                        target.DOKill();
                        target.localScale = originialScale;
                        target.DOScale(originialScale * 1.04f, 0.08f)
                            .SetEase(Ease.OutQuad)
                            .SetLoops(2, LoopType.Yoyo)
                            .SetAutoKill(true)
                            .SetLink(target.gameObject, LinkBehaviour.KillOnDestroy)
                            .OnComplete(() =>
                            {
                                target.localScale = originialScale;
                            });

                        // Play sound effect here if needed
                        //Play haptic effect here if needed
                        coinView.transform.SetParent(coinSourceTransform);
                        coinPool.ReturnToPool(coinView);
                    });

                var coinSeq = DOTween.Sequence()
                    .SetAutoKill(true)
                    .SetLink(coinView.gameObject, LinkBehaviour.KillOnDisable | LinkBehaviour.KillOnDestroy);

                //adding sequences to master sequence, so
                coinSeq.Append(burstSequence);
                coinSeq.Append(moveSequence);

                master.Insert(0f, coinSeq);

            }


            master.Append(DOVirtual.DelayedCall((coinAmount * coinConfig.delayStep) + coinConfig.spawnDuration + coinConfig.moveDuration, () =>
            {
                OnCoinUpdated?.Invoke((int)end);
                target.localScale = originialScale;
                Debug.Log("Coin animation completed");
            })
            .SetAutoKill(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy));

            return master;

        }
    }
}

