using Scripts.GameService;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using Zenject;
using static UnityEngine.ParticleSystem;

namespace Scripts.Particle
{
    public class ParticleService : IParticleService
    {
        public ParticleConfig ParticleConfig { get; private set; }

        public Dictionary<EParticleType, ParticleFXData> ParticleTypes { get; private set; } = new Dictionary<EParticleType, ParticleFXData>();
        private Dictionary<EParticleType ,Queue<ParticleFXView>> ListOfPooledParticles = new();
        private Transform m_ParticleParent;

        private IGameLoopService m_GameLoopService;

        [Inject]
        public void Constuct(ParticleConfig config, IGameLoopService gameLoopService)
        {
            ParticleConfig =  config;
            m_GameLoopService = gameLoopService;

            InitializeDictionary();
            InitalizePool();

        }
        public void InitializeDictionary()
        {
            foreach (ParticleFXData item in ParticleConfig.m_ParticlePrefabs)
            {
                ParticleTypes.Add(item.m_Type, item);
                ListOfPooledParticles.Add(item.m_Type, new());
            }
        }

        private void InitalizePool()
        {
            m_ParticleParent = new GameObject("ParticlePool").transform;
            foreach (ParticleFXData item in ParticleConfig.m_ParticlePrefabs)
            {
                GrowPool(item.m_Type, item.m_MaxParticleCount);
            }
        }

        private void GrowPool(EParticleType eParticleType, int poolCount)
        {

            ParticleFXData particleSystem = GetParticleData(eParticleType);

            for (int i = 0; i < poolCount; i++)
            {
                InstantiateParticle(eParticleType, particleSystem);
            }
        }

        private void InstantiateParticle(EParticleType eParticleType, ParticleFXData data)
        {
            //Instatiate view
            ParticleFXView view = Object.Instantiate(ParticleConfig.m_ParticlePrefabView);
            view.transform.parent = m_ParticleParent;

            view.Setup(ParticleConfig, this, data);
            view.OnParticleCompleted += () => ReturnToPool(eParticleType, view);
            view.Hide();

            AddParticleToList(eParticleType, view);


        }

        private void AddParticleToList(EParticleType particleType, ParticleFXView particle)
        {
            ListOfPooledParticles[particleType].Enqueue(particle);
        }

        public ParticleFXView GetParticle(EParticleType type)
        {
            if (ListOfPooledParticles[type].Count <= 0)
            {
                GrowPool(type, ParticleConfig.m_PoolGrowSize);
            }

            return ListOfPooledParticles[type].Dequeue();
        }

        private ParticleFXData GetParticleData(EParticleType eParticleType)
        {
            return ParticleTypes[eParticleType];
        }

       
        //Takes from the queue list
        public ParticleFXView SpawnParticle(EParticleType type, Vector3 position, Quaternion rotation)
        {
            ParticleFXView front = GetParticle(type); 

            front.transform.position = position;
            front.transform.rotation = rotation;
            front.Show();

            return front;

        }

        // Adds back to the queue list
        public void ReturnToPool(EParticleType type, ParticleFXView particleObject)
        {
            if (particleObject == null) return;

            particleObject.Hide();

            AddParticleToList(type, particleObject);
        }

        public void Cleanup()
        {
            foreach (var particle in ListOfPooledParticles)
            {
                foreach (var item in particle.Value)
                {
                    item.OnParticleCompleted -= () => ReturnToPool(particle.Key, item);

                    Object.Destroy(item);
                }
                particle.Value.Clear();
            }

            ListOfPooledParticles.Clear();

            ParticleTypes.Clear();
        }

    }
}
