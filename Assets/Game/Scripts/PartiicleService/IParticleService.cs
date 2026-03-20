using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Particle
{
    public interface IParticleService
    {
        public ParticleConfig ParticleConfig { get; }
        public Dictionary<EParticleType, ParticleFXData> ParticleTypes { get; }

        public void InitializeDictionary();

        public void Cleanup();

        public ParticleFXView GetParticle(EParticleType type);

        public ParticleFXView SpawnParticle(EParticleType type, Vector3 position, Quaternion rotation);
        public void ReturnToPool(EParticleType type, ParticleFXView partilceObject);
    }
}
