using UnityEngine;

namespace Scripts.UI.Coins
{
    public class CoinView : MonoBehaviour
    {
        [SerializeField] private RectTransform CoinIconRectTransform;

        public RectTransform GetCoinIconRectTransform() => CoinIconRectTransform;
        private void Reset()
        {
            CoinIconRectTransform = GetComponentInChildren<RectTransform>(true);
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
