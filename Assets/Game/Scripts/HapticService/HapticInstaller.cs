using UnityEngine;
using Zenject;

namespace Scripts.Haptics
{
    [CreateAssetMenu(fileName = "HapticInstaller", menuName = "Installers/HapticInstaller")]
    public class HapticInstaller : ScriptableObjectInstaller<HapticInstaller>
    {
        public HapticConfigs HapticConfigs;
        public override void InstallBindings()
        {
            Container.BindInstance(HapticConfigs);
            Container.Bind<IHapticService>().To<HapticService>().AsSingle().NonLazy();
        }
    }
}