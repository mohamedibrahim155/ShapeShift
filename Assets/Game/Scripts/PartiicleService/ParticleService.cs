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
                    item.OnParticleCompleted -= (x) => ReturnToPool(particle.Key, x);

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
            foreach (ParticleFXData item in ParticleConfig.particlePrefabs)
            {
                ParticleTypes.Add(item.particleType, item.particleSystem);
            }
        }

        private void InitalizePool()
        {
            m_ParticleParent = new GameObject("ParticlePool").transform;
            foreach (ParticleFXData item in ParticleConfig.particlePrefabs)
            {
                GrowPool(item.particleType, item.MaxParticleCount);
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
            ParticleFXView view = Object.Instantiate(ParticleConfig.particlePrefabView);
            view.transform.parent = m_ParticleParent;

            ParticleSystem particle = Object.Instantiate(prefab);
            particle.transform.parent = view.transform;

            view.Setup(ParticleConfig, this, particle);

            view.OnParticleCompleted += (x) => ReturnToPool(eParticleType, x);

            AddParticleToList(eParticleType, view);
        }

        private void AddParticleToList(EParticleType particleType, ParticleFXView particle)
        {
            if (!ListOfParticles.ContainsKey(particleType))
            {
                ListOfParticles.Add(particleType, new());
            }

            ListOfParticles[particleType].Enqueue(particle);

            particle.Hide();
        }

        private ParticleSystem GetParticlePrefabByType(EParticleType eParticleType)
        {
            return ParticleTypes[eParticleType];
        }

    

        public bool IsActive(ParticleSystem partilce)
        {
            return partilce.gameObject.activeSelf;
        }

        public void SpawnFX(EParticleType type, Vector3 position, Quaternion rotation)
        {
            ParticleFXView front = GetParticle(type); 

            front.transform.position = position;
            front.transform.rotation = rotation;
            front.Show();
            Debug.Log("Displayed particle");

        }

        public void ReturnToPool(EParticleType type, ParticleFXView particleObject)
        {
            if (particleObject == null) return;


            AddParticleToList(type, particleObject);
        }
        
    }
}
