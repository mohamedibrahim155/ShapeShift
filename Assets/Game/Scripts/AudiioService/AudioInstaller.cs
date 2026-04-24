using UnityEngine;
using Zenject;

namespace Scripts.Audio
{
    [CreateAssetMenu(fileName = "AudioInstaller", menuName = "Installers/AudioInstaller")]
    public class AudioInstaller : ScriptableObjectInstaller<AudioInstaller>
    {
        public AudioConfig Config;
        public override void InstallBindings()
        {
            Container.BindInstance(Config);
            Container.Bind<IAudioService>().To<AudioService>().AsSingle().NonLazy();
        }
    }
}