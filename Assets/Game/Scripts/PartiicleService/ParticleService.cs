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

        public Dictionary<EParticleType, ParticleSystem> ParticleTypes { get; private set; } = new Dictionary<EParticleType, ParticleSystem>();
        private Dictionary<EParticleType ,Queue<ParticleFXView>> ListOfParticles = new();
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

        public void Cleanup()
        {
            foreach (var particle in ListOfParticles)
            {
                foreach (var item in particle.Value)
                {
                    item.OnParticleCompleted -= () => ReturnToPool(particle.Key, item);

                    Object.Destroy(item);
                }
                particle.Value.Clear();
            }

            ListOfParticles.Clear();

            ParticleTypes.Clear();
        }

        public ParticleFXView GetParticle(EParticleType type)
        {
            if (ListOfParticles[type].Count <= 0)
            {
                GrowPool(type, 10);
            }

            return ListOfParticles[type].Dequeue();
        }

        public void InitializeDictionary()
        {
            foreach (ParticleFXData item in ParticleConfig.m_ParticlePrefabs)
            {
                ParticleTypes.Add(item.m_Type, item.m_ParticleSystem);
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

            ParticleSystem particleSystem = GetParticlePrefabByType(eParticleType);

            for (int i = 0; i < poolCount; i++)
            {
                InstantiateParticle( eParticleType, particleSystem);
            }
        }

        private void InstantiateParticle(EParticleType eParticleType, ParticleSystem prefab)
        {
            //Instatiate view
            ParticleFXView view = Object.Instantiate(ParticleConfig.m_ParticlePrefabView);
            view.transform.parent = m_ParticleParent;

            //Instatiate Particle and Assign that to view
            ParticleSystem particle = Object.Instantiate(prefab);
            particle.transform.parent = view.transform;

            view.Setup(ParticleConfig, this, particle);

            view.OnParticleCompleted += () => ReturnToPool(eParticleType, view);

            AddParticleToList(eParticleType, view);


            view.Hide();
        }

        private void AddParticleToList(EParticleType particleType, ParticleFXView particle)
        {
            if (!ListOfParticles.ContainsKey(particleType))
            {
                ListOfParticles.Add(particleType, new());
            }

            ListOfParticles[particleType].Enqueue(particle);

        }

        private ParticleSystem GetParticlePrefabByType(EParticleType eParticleType)
        {
            return ParticleTypes[eParticleType];
        }

    

        public bool IsActive(ParticleSystem partilce)
        {
            return partilce.gameObject.activeSelf;
        }

        public ParticleFXView SpawnParticle(EParticleType type, Vector3 position, Quaternion rotation)
        {
            ParticleFXView front = GetParticle(type); 

            front.transform.position = position;
            front.transform.rotation = rotation;
            front.Show();

            return front;

        }

        public void ReturnToPool(EParticleType type, ParticleFXView particleObject)
        {
            if (particleObject == null) return;

            particleObject.Hide();

            AddParticleToList(type, particleObject);
        }
        
    }
}
