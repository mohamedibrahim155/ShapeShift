using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using System.Diagnostics.Contracts;

namespace Scripts.Shop
{
    public class ShopWindow : UIWindow
    {
        [SerializeField] private List<SkinIItemUIView> SkinItems = new List<SkinIItemUIView>();
        [SerializeField] private ButtonVisuals CloseButton;
        [SerializeField] private GridLayoutGroup SkinPageGridContent;

        public event Action<SkinIItemUIView> OnSkinItemClicked = delegate { };
        public event Action OnCloseButtonClicked = delegate { };

        public override void Reset()
        {
            base.Reset();
            ID = EWindowID.Shop;
            SkinItems = GetComponentsInChildren<SkinIItemUIView>(true).ToList();
            CloseButton = GetComponentInChildren<ButtonVisuals>(true);
            SkinPageGridContent = GetComponentInChildren<GridLayoutGroup>(true);
        }

        private void OnEnable()
        {

            foreach (var item in SkinItems)
            {
                item.OnSkinItemClicked += HandleClickedItem;
            }

            CloseButton.Button.onClick.AddListener(HandleCloseButtonClicked);

        }

        private void HandleCloseButtonClicked()
        {
            OnCloseButtonClicked.Invoke();
        }

        private void OnDisable()
        {

            foreach (var item in SkinItems)
            {
                item.OnSkinItemClicked -= HandleClickedItem;
            }

            CloseButton.Button.onClick.RemoveListener(HandleCloseButtonClicked);
        }



        private void HandleClickedItem(SkinIItemUIView itemUIView)
        {
            OnSkinItemClicked.Invoke(itemUIView);
        }

        public void InitalizeSkinUI(int skinSize, SkinIItemUIView prefabToSpawn)
        {
            DestroyContentUI();
       

            for (int i = 0; i < skinSize; i++)
            {
                SkinIItemUIView uIView = Instantiate(prefabToSpawn, SkinPageGridContent.transform);
                uIView.OnSkinItemClicked += HandleClickedItem;
                SkinItems.Add(uIView);
            }
        }

        public void UpdateData(int index, Sprite skin)
        {
            if (SkinItems.Count  > index)
            {
                SkinItems[index].SetSkin(skin);
            }
        }
        public void DeselectAllSkin()
        {
            foreach (var item in SkinItems)
            {
                item.Deselected();
            }
        }

        public void DestroyContentUI()
        {
            if (SkinItems.Count == 0) return;

            for (int i = 0; i < SkinItems.Count; i++)
            {
                Destroy(SkinItems[i].gameObject);
            }

            SkinItems.Clear();
        }

    }
}
