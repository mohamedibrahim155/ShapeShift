using Scripts.Player;
using Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.UI
{
    public class MainMenu : UIWindow
    {
        [SerializeField] private Button PlayButton;
        [SerializeField] private Button OptionButton;
        [SerializeField] private Button QuitButton;
        [SerializeField] private Button ShopButton;

        public event Action OnPlayClicked = delegate { };
        public event Action OnSettingsClicked = delegate { };
        public event Action OnQuitClicked = delegate { };
        public event Action OnShopClicked = delegate { };
        private void Awake()
        {
            PlayButton.onClick.AddListener(() => OnPlayClicked.Invoke());
            OptionButton.onClick.AddListener(() => OnSettingsClicked.Invoke());
            QuitButton.onClick.AddListener(() => OnQuitClicked.Invoke());
            ShopButton.onClick.AddListener(() => OnShopClicked.Invoke());
        }

        private void OnDestroy()
        {
            PlayButton.onClick.RemoveAllListeners();
            OptionButton.onClick.RemoveAllListeners();
            QuitButton.onClick.RemoveAllListeners();
            ShopButton.onClick.RemoveAllListeners();
        }

    }
}
