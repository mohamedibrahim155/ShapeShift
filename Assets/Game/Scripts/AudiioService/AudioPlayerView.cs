using System;
using System.Collections;
using UnityEngine;

namespace Scripts.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayerView : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        

        public event Action<AudioPlayerView> OnFinishedPlaying = delegate { };
        private void Reset()
        {
            _audioSource =GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip, float volume, float pitch,bool loop, bool spatial, Vector3? position, float minDistance, float maxDistance)
        {
            if (_audioSource == null || clip == null) return;

            if (position.HasValue)
            {
                transform.position = position.Value;
            }

            _audioSource.clip = clip;
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
            _audioSource.loop = loop;
            _audioSource.spatialBlend = spatial ? 1f : 0f;
            _audioSource.minDistance = minDistance;
            _audioSource.maxDistance = maxDistance;
            _audioSource.Play();
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public bool IsPlaying()
        {
            return _audioSource.isPlaying;
        }

        public void ResetState()
        {
            _audioSource.clip = null;
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;
            _audioSource.volume = 1f;
            _audioSource.pitch = 1f;
            _audioSource.spatialBlend = 0f;
        }

        public void StartTick()
        {
            StartCoroutine(WaitForPlaybackToFinish());
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);


        public IEnumerator WaitForPlaybackToFinish()
        { 

            while (_audioSource != null && _audioSource.isPlaying)
            {
                yield return null;
            }

            if (_audioSource != null)
            {
                OnFinishedPlaying.Invoke(this);
            }
        }
    }
}
