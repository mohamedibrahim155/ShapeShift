using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Unity.VisualScripting.Member;

namespace Scripts.Audio
{
    public class AudioService : IAudioService
    {
        private AudioPool m_AudioPool;
        public AudioConfig Config { get; private set; }

        private readonly Dictionary<string, AudioData> _audioKeyData = new Dictionary<string, AudioData>(StringComparer.Ordinal);

        private AudioPlayerView musicPlayerSource;

        [Inject]
        public void Contruct(AudioConfig config)
        {
            Config = config;
            Initalize();
        }

        #region Initialization Methods
        private void Initalize()
        {
            Config.LoadAudioSettings();
            RegisterAudioData();
            IntializePool();
            EnsureMusicPlayerSource();
        }

  
        private void RegisterAudioData()
        {
            _audioKeyData.Clear();
            foreach (var audioData in Config.AudioDatas)
            {
                if (audioData == null || audioData.AudioClip == null) continue;
                if(string.IsNullOrEmpty(audioData.Key)) continue;

                _audioKeyData.Add(audioData.Key, audioData);
            }
        }
        private void IntializePool()
        {
            m_AudioPool = new AudioPool.Builder()
                  .SetPrefab(Config.AudioPlayerViewPrefab)
                  .SetInitialPoolSize(10)
                  .SetParent(new GameObject("AudioPool Holder").transform) // can cache  the transform
                  .Build();

            m_AudioPool.Initialize();
        }
        private void EnsureMusicPlayerSource()
        {
            if (musicPlayerSource == null)
            {
                musicPlayerSource = m_AudioPool.GetAudio();
                musicPlayerSource.transform.SetParent(null);
                musicPlayerSource.name = " BGM_MUSIC";
            }
        }
        #endregion

        public bool IsEnabled()
        {
            return Config.IsMusicEnabled;
        }

        /// <summary>
        /// Plays sounds that are not spatial (2D)
        /// </summary>
        /// <param name="key"></param>
        public void Play(string key)
        {
            PlayInternal(key, false, Vector3.zero);
        }
        /// <summary>
        /// Plays sounds at a specific world position. 
        /// <param name="key"></param>
        /// <param name="worldPosition"></param>
        public void PlayAt(string key, Vector3 worldPosition)
        {
            PlayInternal(key, true, worldPosition);
        }

        // Plays musics that are meant to be background music (BGM).
        public void PlayMusicBGM(string key)
        {
            if (!IsEnabled()) return;
            if (!_audioKeyData.TryGetValue(key, out var data) || data == null || data.AudioClip == null)
            {
                return;
            }
            musicPlayerSource.Play(data.AudioClip,
                data.Volume,
                data.Pitch,
                true, // Loop music
                false, // Music is not spatial
                null, // No position for music
                0f, // Min distance not relevant for music
                0f); // Max distance not relevant for music
        }

        /// <summary>
        /// Enables or disables music playback.
        /// When disabled, it also stops any currently playing music.
        /// </summary>
        /// <param name="enabled"></param>
        public void SetMusicEnabled(bool enabled)
        {
            Config.SetEnabled(enabled);

            if (!enabled)
            {
                StopMusic();
            }
        }


        /// <summary>
        /// Stops the currently playing music, if any.
        /// </summary>
        public void StopMusic()
        {
            if (musicPlayerSource == null) return;

            musicPlayerSource.Stop();
        }

        private void PlayInternal(string key, bool is3D, Vector3 position)
        {
            if (!IsEnabled()) return;

            if (!_audioKeyData.TryGetValue(key, out var data)  || data == null || data.AudioClip ==  null)
            {
                return;
            }

            AudioPlayerView source = m_AudioPool.GetAudio();


            if (source == null) return;
            Vector3? pos = is3D ? position : (Vector3?)null;

            bool spatial = data.Spatial || is3D;

            source.Play(data.AudioClip,
                data.Volume,
                data.Pitch,
                data.Loop,
                 spatial,
                pos,
                data.MinDistance,
                data.MaxDistance);

            if (!data.Loop)
            {
                source.StartTick();
                source.OnFinishedPlaying += RecycleAfterFinishPlaying;
            }

        }

        /// <summary>
        ///  Returns the audio player to the pool after it finishes playing a non-looping sound.
        /// </summary>
        /// <param name="player"></param>
        private void RecycleAfterFinishPlaying(AudioPlayerView player)
        {
            m_AudioPool.ReturnToPool(player);
            player.OnFinishedPlaying -= RecycleAfterFinishPlaying;
        }

        public void Cleanup()
        {
            m_AudioPool.Cleanup();

            musicPlayerSource.Stop();
            UnityEngine.Object.Destroy(musicPlayerSource.gameObject);
        }
    }
}
