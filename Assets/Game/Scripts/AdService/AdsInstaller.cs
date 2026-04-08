using Scripts.Ads;
using UnityEngine;
using Zenject;

namespace Scripts.Ads
{

    [CreateAssetMenu(fileName = "AdsIntaller", menuName = "Installers/AdsIntaller")]
    public class AdsInstaller : ScriptableObjectInstaller<AdsInstaller>
    {
        public AdsConfig AdsConfig;
        public override void InstallBindings()
        {
            Container.BindInstance(AdsConfig);
            Container.Bind<IAdService>().To<AdService>().AsSingle().NonLazy();
           // Container.Bind<RewardedAdController>().ToSelf().AsSingle().NonLazy();
        }
    }
}