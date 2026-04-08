using Scripts.UI;
using System;
using UnityEngine;

namespace Scripts.Ads
{
    [RequireComponent(typeof(ButtonVisuals))]
    public class RewardedAdButtonView : MonoBehaviour
    {
        [SerializeField] private ButtonVisuals rewardedButtonVisual;

        public event Action OnButtonClicked = delegate { };


        private void Awake()
        {
            rewardedButtonVisual.Button.onClick.AddListener(HandleOnClick);
        }

        public void SetInteractable( bool interactable)
        {
            rewardedButtonVisual.Button.interactable = interactable;
        }


        private void HandleOnClick()
        {
            OnButtonClicked.Invoke();
        }

        private void Reset()
        {
            rewardedButtonVisual = GetComponent<ButtonVisuals>();
        }

        private void OnDestroy()
        {
            rewardedButtonVisual.Button.onClick.RemoveListener(HandleOnClick);
        }
    }
}
