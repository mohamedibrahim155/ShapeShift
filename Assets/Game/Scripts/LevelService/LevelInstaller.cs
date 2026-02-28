using UnityEngine;
using Zenject;
using System.Collections.Generic;

namespace Scripts.Level
{

    [CreateAssetMenu(fileName = "LevelInstaller", menuName = "Installers/LevelInstaller")]
    public class LevelInstaller : ScriptableObjectInstaller<LevelInstaller>
    {
        public List<LevelConfig> Levelsconfig;
        public override void InstallBindings()
        {
            Container.BindInstances(Levelsconfig);
            Container.Bind<ILevelService>().To<LevelService>().AsSingle().NonLazy();
        }
    }
}