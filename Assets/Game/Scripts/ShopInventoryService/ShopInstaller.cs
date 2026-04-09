using UnityEngine;
using Zenject;

namespace Scripts.Shop
{

    [CreateAssetMenu(fileName = "ShopInstaller", menuName = "Installers/ShopInstaller")]
    public class ShopInstaller : ScriptableObjectInstaller<ShopInstaller>
    {
        public ShopSkinConfig shopSkinConfig;
        public override void InstallBindings()
        {
            Container.BindInstance(shopSkinConfig);
            Container.Bind<IShopService>().To<ShopService>().AsSingle().NonLazy();
        }
    }
}