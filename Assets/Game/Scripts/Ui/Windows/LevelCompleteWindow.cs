using DG.Tweening;
using JetBrains.Annotations;
using Scripts.Level;
using Scripts.Player;
using Scripts.UI.Coins;
using Scripts.Utilities.Pool;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Scripts.UI
{
    public class LevelCompleteWindow : UIWindow
    {
        [Header("Buttons")]
        [SerializeField] private ButtonVisuals NextLevelButton;
        [SerializeField] private ButtonVisuals RemoveAdsButton;

        [Header("Text labels")]
        [SerializeField] private TextMeshProUGUI LevelNumberTextField;
        [SerializeField] private TextMeshProUGUI TotalCoinsTextField;
        [SerializeField] private TextMeshProUGUI CoinRewardTextField;


        [Header("Coin Reward Animation")]
        [SerializeField] private CoinAnimationView coinRewardAnimationView;
        [SerializeField] private RectTransform CoinSourceRect;
        [SerializeField] private RectTransform CoinTargetRect;

        // Events
        public Action OnNextLevelClicked = delegate { };
        public Action OnRemoveAdsClicked = delegate { };

        private Sequence _previousCoinSequence;


        private void Awake()
        {
            NextLevelButton.Button.onClick.AddListener(() => OnNextLevelClicked.Invoke());
            RemoveAdsButton.Button.onClick.AddListener(() => OnRemoveAdsClicked.Invoke());
        }
        private IEnumerator DelayOpenCallback(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Open();
        }
        public void OpenLevelCompleteScreenWithDelay()
        {
            StartCoroutine(DelayOpenCallback(1));
        }

        public void UpdateLevelText(int levelNumber)
        {
            LevelNumberTextField.text = $"Level {levelNumber}";
        }

        private void OnDestroy()
        {
            NextLevelButton.Button.onClick.RemoveAllListeners();
            RemoveAdsButton.Button.onClick.RemoveAllListeners();
        }

        public void PlayCoinRewardAnimation(int startAmount, int addAmount)
        {
            if (_previousCoinSequence != null && _previousCoinSequence.IsActive()) return;

            _previousCoinSequence = coinRewardAnimationView.AddCoinAnimate(CoinSourceRect, CoinTargetRect,
                (coins) =>
                {
                    UpdateTotalCoins(coins);
                }, startAmount, addAmount)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(() =>
                {
                    Debug.Log("Proccess next level after reward");

                    UpdateTotalCoins(startAmount + addAmount);
                });
        }

        [ContextMenu("Test_Coin_Animation")]
        public void TestAnimation()
        {
            int lastAmount = int.Parse(TotalCoinsTextField.text);
            int coinAdded = UnityEngine.Random.Range(10, 20);
            PlayCoinRewardAnimation(lastAmount, coinAdded);
        }

        public void UpdateTotalCoins(int coins)
        {
            TotalCoinsTextField.text = coins.ToString();
        }

        public void UpdateRewardCoins(int receivedCoins)
        {
            CoinRewardTextField.text = receivedCoins.ToString();
        }

        //public void PlayCoinAnimation(int startAmount, int addAmount)
        //{
        //    if (_previousCoinSequence != null && _previousCoinSequence.IsActive()) return;


        //    _previousCoinSequence =  AddCoinAnimate(CoinSourceRect, totalCoinTargetRect, startAmount, addAmount);

        //    if (_previousCoinSequence != null)
        //    {
        //        _previousCoinSequence
        //            .SetAutoKill(true)
        //            .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
        //            .OnComplete(() =>
        //            {
        //                Debug.Log("Proccess next level after reward");
        //            });

        //        return;
        //    }
        //}

        //[ContextMenu("TestCoinAnimation")]
        //public void TestAnimation()
        //{
        //    PlayCoinAnimation(0, 20);
        //}

        //private Sequence AddCoinAnimate(RectTransform source, RectTransform target, long startAmount, long addAmount)
        //{
        //    if (coinPool == null ||  config  ==  null ||  source == null || target == null)
        //        return null;

        //    int coinAmount = Mathf.Clamp((int)addAmount, 2, 12);
        //    long end = startAmount + addAmount;


        //    var master = DOTween.Sequence()
        //     .SetAutoKill(true)
        //     .SetLink(gameObject, LinkBehaviour.KillOnDestroy);


        //    Vector3 originialScale = target.localScale;



        //    Transform globalParent = null;
        //    for (int i = 0; i < coinAmount; i++)
        //    {
        //        CoinView coinTransform = coinPool.GetCoin();

        //        //cache parent
        //        if (globalParent ==  null)
        //        {
        //            globalParent = coinTransform.transform.parent;
        //        }
        //        RectTransform coinRect = coinTransform.GetCoinIconRectTransform();
        //        coinRect.position = source.position;
        //        coinTransform.transform.SetParent(target);




        //        float t =  i / (float)(coinAmount -1);


        //        Vector2 burstOffset =  UnityEngine.Random.insideUnitCircle * config.spawnRadius;

        //        //burst
        //        var burstSequence = coinRect.DOMove(source.position + (Vector3)burstOffset, config.spawnDuration + config.globalDelay).
        //            SetEase(Ease.InOutSine).
        //            SetDelay(t * config.delayStep).
        //            SetAutoKill(true).
        //            SetLink(coinTransform.gameObject, LinkBehaviour.KillOnDisable | LinkBehaviour.KillOnDestroy);

        //        //Move animation
        //        var moveSequence = coinRect.DOMove(target.position, config.moveDuration + config.globalDelay).
        //            SetEase(Ease.InBack).
        //            SetAutoKill(true).
        //            SetLink(coinTransform.gameObject, LinkBehaviour.KillOnDisable | LinkBehaviour.KillOnDestroy).
        //            OnComplete(() => 
        //            {
        //                long stepValue = (long)Mathf.Ceil(Mathf.Lerp(startAmount, end, t));

        //               // totalText.text = stepValue.ToString();
        //                target.DOKill();
        //                target.localScale = originialScale;
        //                target.DOScale(originialScale * 1.04f, 0.08f)
        //                    .SetEase(Ease.OutQuad)
        //                    .SetLoops(2, LoopType.Yoyo)
        //                    .SetAutoKill(true)
        //                    .SetLink(target.gameObject,LinkBehaviour.KillOnDestroy)
        //                    .OnComplete(() => 
        //                    { 
        //                        target.localScale = originialScale;
        //                    });

        //                // Play sound effect here if needed
        //                //Play haptic effect here if needed
        //                coinTransform.transform.SetParent(globalParent);
        //                coinPool.ReturnToPool(coinTransform);
        //            });

        //        var coinSeq = DOTween.Sequence()
        //            .SetAutoKill(true)
        //            .SetLink(coinTransform.gameObject, LinkBehaviour.KillOnDisable | LinkBehaviour.KillOnDestroy);

        //        //adding sequences to master sequence, so
        //        coinSeq.Append(burstSequence);
        //        coinSeq.Append(moveSequence);

        //        master.Insert(0f, coinSeq);

        //    }


        //    master.Append(DOVirtual.DelayedCall(coinAmount * config.delayStep + config.spawnDuration + config.moveDuration, () =>
        //    {
        //      //  totalText.text = end.ToString();
        //        target.localScale = originialScale;
        //    })
        //    .SetAutoKill(true)
        //    .SetLink(gameObject, LinkBehaviour.KillOnDestroy));

        //    return master;

        //}

    }
}
