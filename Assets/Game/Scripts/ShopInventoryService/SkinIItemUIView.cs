using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace Scripts.Shop
{
    public class SkinIItemUIView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image BackgroundImage;
        [SerializeField] private Image SkinImage;

        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _defaultColor;

        public Action<SkinIItemUIView> OnSkinItemClicked = delegate { };



        private void Awake()
        {
            ResetData();
        }

        public void SetSelectedColor(Color color)
        {
            _selectedColor = color;
        }
        public void SetDefaultColor(Color color)
        {
            _defaultColor = color;
        }

        private void ResetData()
        {
            BackgroundImage.color = _defaultColor;
        }

        public void SetSkin(Sprite skinSprite)
        {
            SkinImage.sprite = skinSprite;
        }
        public void Selected()
        {
            BackgroundImage.color = _selectedColor;
        }

        public void Deselected()
        {
            ResetData();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData != null)
            {
                OnSkinItemClicked.Invoke(this);
            }
        }
    }
}
