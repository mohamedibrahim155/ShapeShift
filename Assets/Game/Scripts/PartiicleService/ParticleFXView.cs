using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Particle
{
    public class ParticleFXView : MonoBehaviour
    {
        public event Action<ParticleFXView> OnParticleCompleted = delegate { };
        public bool m_IsAlive { get; private set; }


        [SerializeField] private  ParticleSystem _particleSystem;

        private ParticleConfig _config;
        private IParticleService _particleService;

        private void Awake()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            m_IsAlive = true;
            _particleSystem.Play();

            float delayTime = CalculateTotalLifetime();
            StartCoroutine(WaitForParticleCompletion(delayTime));
        }

        private IEnumerator WaitForParticleCompletion(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            OnParticleCompleted.Invoke(this);
        }

        public void Hide()
        {
            _particleSystem.Stop();
            m_IsAlive = false;
            gameObject.SetActive(false);
        }

        private float CalculateTotalLifetime()
        {
            float maxTotalTime = _config.m_MinLifeTime;
            maxTotalTime = Mathf.Max(maxTotalTime, _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax);
            return maxTotalTime;
        }

        public void Setup(ParticleConfig particleConfig, IParticleService particleService, ParticleSystem particleSystem)
        {
            _config = particleConfig;
            _particleService = particleService;
            this._particleSystem = particleSystem;
        }

    }
}
