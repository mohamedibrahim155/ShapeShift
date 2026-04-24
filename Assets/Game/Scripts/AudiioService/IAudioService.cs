using UnityEngine;

namespace Scripts.Audio
{
    public interface IAudioService
    {
        AudioConfig Config { get; }
        void Play(string key);
        void PlayAt(string key, Vector3 worldPosition);
        void SetMusicEnabled(bool enabled);
        bool IsEnabled();
        void Cleanup();
    }
}
