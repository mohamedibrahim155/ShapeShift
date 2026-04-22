using Scripts.UI.Coins;
using UnityEngine;
using Zenject;

namespace Scripts.UI.Coins
{
    [CreateAssetMenu(fileName = "CoinInstaller", menuName = "Installers/CoinInstaller")]
    public class CoinInstaller : ScriptableObjectInstaller<CoinInstaller>
    {
        public CoinConfig config;
        public override void InstallBindings()
        {
            Container.BindInstance(config);
            Container.Bind<ICoinService>().To<CoinService>().AsSingle().NonLazy();
        }
    }
}