using Scripts.Particle;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ParticleFXInstaller", menuName = "Installers/ParticleFXInstaller")]
public class ParticleFXInstaller : ScriptableObjectInstaller<ParticleFXInstaller>
{
    public ParticleConfig ParticleConfig;
    public override void InstallBindings()
    {
        Container.BindInstances(ParticleConfig);
        Container.Bind<IParticleService>().To<ParticleService>().AsSingle().NonLazy();
    }
}