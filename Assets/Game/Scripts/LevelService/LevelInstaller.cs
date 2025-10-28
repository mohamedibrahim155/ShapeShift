using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "LevelInstaller", menuName = "Installers/LevelInstaller")]
public class LevelInstaller : ScriptableObjectInstaller<LevelInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ILevelService>().To<LevelService>().AsSingle().NonLazy();
    }
}