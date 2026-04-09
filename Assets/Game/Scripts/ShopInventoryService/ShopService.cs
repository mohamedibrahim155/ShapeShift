using Scripts.GameService;
using Scripts.UI;
using System;
using UnityEngine;
using Zenject;

namespace Scripts.Shop
{
    public class ShopService : IShopService
    {
        public ShopSkinConfig ShopConfig { get; private set; }
        public ShopWindow ShopWindowView { get; private set; }

        private IUIService m_uiService;
        private IGameLoopService m_gameLoopService;


        [Inject]
        public void Contruct(ShopSkinConfig config, IUIService uIService, IGameLoopService gameLoopService)
        {
            ShopConfig = config;
            m_uiService = uIService;
            m_gameLoopService = gameLoopService;

            ShopWindowView = m_uiService.GetWindow(EWindowID.Shop) as ShopWindow;

            m_gameLoopService.OnDestroyed += CleanUp;

            InitalizeUI();
        }

        private void CleanUp()
        {
            ShopWindowView.OnSkinItemClicked    -= HandleSkinItemClicked;
            ShopWindowView.OnCloseButtonClicked -= HandleCloseButtonClicked;
        }

        private void InitalizeUI()
        {
            ShopWindowView.InitalizeSkinUI(ShopConfig.Size, ShopConfig.SkinUIView);

            ShopWindowView.OnSkinItemClicked    += HandleSkinItemClicked;
            ShopWindowView.OnCloseButtonClicked += HandleCloseButtonClicked;
        }

        public void HadleOpenShop()
        {
            ShopWindowView.Open();
        }

        private void HandleCloseButtonClicked()
        {
            ShopWindowView.Close();
        }

        private void HandleSkinItemClicked(SkinIItemUIView view)
        {
            ShopWindowView.DeselectAllSkin();
            view.Selected();
        }
    }
}
