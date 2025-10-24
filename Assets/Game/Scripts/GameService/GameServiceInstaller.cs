using UnityEngine;
using Zenject;

namespace Scripts.GameService
{

    [CreateAssetMenu(fileName = "GameServiceInstaller", menuName = "Installers/GameServiceInstaller")]
    public class GameServiceInstaller : ScriptableObjectInstaller<GameServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IGameService>().To<GameService>().AsSingle().NonLazy(); 
        }
    }
}