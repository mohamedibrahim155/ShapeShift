using Scripts.GameService;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameloopInstaller", menuName = "Installers/GameloopInstaller")]
public class GameloopInstaller : ScriptableObjectInstaller<GameloopInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IGameLoopService>().To<GameLoopService>().AsSingle().NonLazy();
    }
}