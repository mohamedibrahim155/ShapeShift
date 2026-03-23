using UnityEngine;
using Zenject;
namespace Scripts.SkyService
{
    [CreateAssetMenu(fileName = "SkyboxInstaller", menuName = "Installers/SkyboxInstaller")]
    public class SkyboxInstaller : ScriptableObjectInstaller<SkyboxInstaller>
    {
        public SkyboxConfig SkyboxConfig;
        public override void InstallBindings()
        {
            Container.BindInstance(SkyboxConfig);
            Container.Bind<ISkyService>().To<SkyService>().AsSingle().NonLazy();
        }
    }
}
