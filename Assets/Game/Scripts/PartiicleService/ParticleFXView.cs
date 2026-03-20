using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Particle
{
    public class ParticleFXView : MonoBehaviour
    {
        public event Action OnParticleCompleted = delegate { };
        public bool m_IsAlive { get; private set; }


        [SerializeField] private  ParticleSystem _particleSystem;

        private ParticleConfig _config;
        private IParticleService _particleService;
        private ParticleFXData _particleFXData;

        public void Setup(ParticleConfig particleConfig, IParticleService particleService, ParticleFXData particleFXData)
        {
            _config = particleConfig;
            _particleService = particleService;
            _particleFXData = particleFXData;

            SpawnParticle();
        }

        private void SpawnParticle()
        {
            _particleSystem = ParticleSystem.Instantiate(_particleFXData.m_ParticleSystem);
            _particleSystem.transform.parent = transform;
        }
        public void Show()
        {
            _particleSystem.transform.localScale = Vector3.one * _particleFXData.m_ScaleFactor;

            gameObject.SetActive(true);
            m_IsAlive = true;
            _particleSystem.Play();

            //Triggers when the particle starts to play
            float delayTime = CalculateTotalLifetime();
            StartCoroutine(WaitForParticleCompletion(delayTime));
        }
        public void Hide()
        {
            _particleSystem.Stop();
            m_IsAlive = false;
            gameObject.SetActive(false);
        }
        private IEnumerator WaitForParticleCompletion(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            OnParticleCompleted.Invoke();
        }

        private float CalculateTotalLifetime()
        {
            float maxTotalTime = _config.m_MinLifeTime;
            maxTotalTime = Mathf.Max(maxTotalTime, _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax);
            return maxTotalTime;
        }

       

    }
}
