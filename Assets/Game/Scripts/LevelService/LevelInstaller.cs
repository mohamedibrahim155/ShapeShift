using UnityEngine;
using Zenject;

namespace Scripts.Level
{

    [CreateAssetMenu(fileName = "LevelInstaller", menuName = "Installers/LevelInstaller")]
    public class LevelInstaller : ScriptableObjectInstaller<LevelInstaller>
    {
        public LevelConfig config;
        public override void InstallBindings()
        {
            Container.BindInstances(config);
            Container.Bind<ILevelService>().To<LevelService>().AsSingle().NonLazy();
        }
    }
}