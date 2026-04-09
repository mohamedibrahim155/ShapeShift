using UnityEngine;
namespace Scripts.Shop
{
    [CreateAssetMenu(fileName = "ShopSkinConfig", menuName = "Scriptable Objects/Shop/ShopSkinConfig")]
    public class ShopSkinConfig : ScriptableObject
    {
        public SkinIItemUIView SkinUIView;
        public int Size;
        public ShopSkinData[] shopSkins;

        public ShopSkinData GetSkinDataByID(int id)
        {
            foreach (var skinData in shopSkins)
            {
                if (skinData.skinId == id)
                {
                    return skinData;
                }
            }
            Debug.LogWarning($"Skin with ID {id} not found. Returning default skin data.");
            return ShopSkinData.CreateDefault();
        }
    }

    [System.Serializable]
    public struct ShopSkinData
    {
        public int skinId;
        public string skinName;
        public int price;
        public Sprite skinSprite;

        public static ShopSkinData Create(int id, string name, int price, Sprite sprite)
        {
            return new ShopSkinData
            {
                skinId = id,
                skinName = name,
                price = price,
                skinSprite = sprite
            };
        }

        public static ShopSkinData CreateDefault()
        {
            return new ShopSkinData
            {
                skinId = 0,
                skinName = "Default Skin",
                price = 0,
                skinSprite = null
            };
        }
    }
}
