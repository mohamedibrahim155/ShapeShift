using Scripts.Score;
using UnityEngine;
using Zenject;


namespace Scripts.Score
{
    [CreateAssetMenu(fileName = "ScoreInstaller", menuName = "Installers/ScoreInstaller")]
    public class ScoreInstaller : ScriptableObjectInstaller<ScoreInstaller>
    {
        public ScoreConfig Config;
        public override void InstallBindings()
        {
            Container.BindInstances(Config);
            Container.Bind<IScoreService>().To<ScoreService>().AsSingle().NonLazy();
        }
    }
}