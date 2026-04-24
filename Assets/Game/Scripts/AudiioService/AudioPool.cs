
using Scripts.UI.Coins;
using Scripts.Utilities.Pool;
using UnityEngine;

namespace Scripts.Audio
{
    public class AudioPool : GenericPool<AudioPlayerView>
    {
        private Transform parent;
        public AudioPool(AudioPlayerView prefab, int initialPoolSize = 10, Transform parent = null) : base(prefab, initialPoolSize)
        {
            this.parent = parent;
        }

        protected override AudioPlayerView CreateInstance()
        {
            AudioPlayerView instance = base.CreateInstance();
            instance.transform.SetParent(parent);
            instance.Hide();
            return instance;
        }

        public AudioPlayerView GetAudio()
        {
            AudioPlayerView audio = Get();
            audio.Show();
            return audio;
        }

        public override void ReturnToPool(AudioPlayerView item)
        {
            item.Hide();
            item.ResetState();
            base.ReturnToPool(item);
        }

        internal class Builder
        {
            private AudioPlayerView prefab;
            private Transform parent;
            private int initialPoolSize = 10;
            internal Builder SetPrefab(AudioPlayerView prefab)
            {
                this.prefab = prefab;
                return this;
            }
            internal Builder SetInitialPoolSize(int size)
            {
                this.initialPoolSize = size;
                return this;
            }
             internal Builder SetParent(Transform parent)
            {
                this.parent = parent;
                return this;
            }
            internal AudioPool Build()
            {
                if (prefab == null)
                {
                    Debug.LogError("AudioPlayerView prefab is not set for AudioPool.");
                    return null;
                }
                return new AudioPool(prefab, initialPoolSize,parent);
            }

        }
    }
}
