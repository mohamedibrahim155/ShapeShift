using Scripts.UI;
using UnityEngine;

namespace Scripts.Shop
{
    public interface IShopService
    {
        public ShopSkinConfig ShopConfig { get; }
        public ShopWindow ShopWindowView { get; }
        public void HadleOpenShop();
    }
}
