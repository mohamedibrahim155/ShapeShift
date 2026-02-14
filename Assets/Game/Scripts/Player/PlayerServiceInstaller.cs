using Scripts.Score;
using UnityEngine;
using Zenject;

namespace Scripts.Player
{
    [CreateAssetMenu(fileName = "PlayerService", menuName = "Installers/PlayerService")]
    public class PlayerServiceInstaller : ScriptableObjectInstaller<PlayerServiceInstaller>
    {
        public PlayerConfig config;
        public override void InstallBindings()
        {
            Container.BindInstance(config);
            Container.Bind<IPlayerService>().To<PlayerService>().AsSingle().NonLazy();
            Container.Bind<IPLayerInputService>().To<PlayerInputService>().AsSingle().NonLazy();
            Container.Bind<IScoreService>().To<ScoreService>().AsSingle().NonLazy();
        }
    }
}
