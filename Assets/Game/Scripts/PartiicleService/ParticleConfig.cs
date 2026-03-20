using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Particle
{
    [CreateAssetMenu(fileName = "ParticleConfig", menuName = "Scriptable Objects/Configs/ParticleConfig")]
    public class ParticleConfig : ScriptableObject
    {
        public float m_MinLifeTime = 0.5f;
        public ParticleFXView particlePrefabView;
        public List<ParticleFXData> particlePrefabs;
    }

    [System.Serializable]
    public class ParticleFXData
    {
        public EParticleType particleType;
        public ParticleSystem particleSystem;
        public int MaxParticleCount;
    }
}
