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

        private ParticleConfig m_Config;
        private IParticleService m_ParticleService;
        private void Awake()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            m_IsAlive = true;

            PlayParticles();

            float deleyTime = GetTotatlPlayime();

            Debug.Log($"delay time: {deleyTime}");
            StartCoroutine(DelayTimeerCall(deleyTime));
            
        }

        public void Hide()
        {
            StopParticles();
            m_IsAlive = false;
            gameObject.SetActive(false);
        }

        IEnumerator DelayTimeerCall(float DelayTime)
        {
            yield return new WaitForSeconds(DelayTime);
            OnParticleCompleted.Invoke(this);

        }

        private void PlayParticles()
        {

            _particleSystem.Play();
            
        }

        private float GetTotatlPlayime()
        {
            float maxTotalTime = m_Config.m_MinLifeTime;
          
                maxTotalTime  =  Mathf.Max(maxTotalTime, _particleSystem.totalTime);
            

            return maxTotalTime;
        }

        private void StopParticles()
        {

            _particleSystem.Stop();
            
        }

        public void Setup(ParticleConfig particleConfig, IParticleService particleService, ParticleSystem particleSystem)
        {
            m_Config = particleConfig;
            m_ParticleService = particleService;
            this._particleSystem = particleSystem;
        }

    }
}
