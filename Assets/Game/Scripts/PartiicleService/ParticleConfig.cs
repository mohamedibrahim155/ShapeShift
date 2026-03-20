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
        public int m_PoolGrowSize = 10;
        public ParticleFXView m_ParticlePrefabView;
        public List<ParticleFXData> m_ParticlePrefabs;
    }

    [System.Serializable]
    public class ParticleFXData
    {
        public EParticleType m_Type;
        public float m_ScaleFactor = 1;
        public ParticleSystem m_ParticleSystem;
        public int m_MaxParticleCount;
    }
}
