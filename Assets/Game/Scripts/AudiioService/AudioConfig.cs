using UnityEditor;
using UnityEngine;
namespace Scripts.Audio
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Scriptable Objects/Audio/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        [Header("Audio Settings")]
         public bool IsMusicEnabled = true;

        [Header("Playback Configuration")]
        [SerializeField][Min(1)] private int audioPolSize = 10;

        [Header("Prefab")]
        [SerializeField] private AudioPlayerView audioPlayerViewPrefab;

        [Header("Audio Library")]
        [SerializeField] private AudioData[] audioDatas;

        public int AudioPoolSize => audioPolSize;
        public AudioPlayerView AudioPlayerViewPrefab => audioPlayerViewPrefab;
        public AudioData[] AudioDatas => audioDatas;

        // Events
        public event System.Action<bool> OnMusicEnabledChanged = delegate { };


        /// <summary>
        /// Set the music enabled state and invoke the corresponding event. 
        /// Also saves the setting to PlayerPrefs.
        /// </summary>
        /// <param name="isEnabled"></param>
        public void SetEnabled(bool isEnabled)
        {
            IsMusicEnabled = isEnabled;
            OnMusicEnabledChanged.Invoke(isEnabled);
            SaveUtility.SaveData(PlayerPrefsKeys.SettingsSound, isEnabled ? 1 : 0);
        }

        /// <summary>
        /// Loads the music enabled state from PlayerPrefs and applies it.
        /// If no setting is found, defaults to true (enabled).
        /// </summary>

        public void LoadAudioSettings()
        {
            int hasEnabled = (int)SaveUtility.LoadData(PlayerPrefsKeys.SettingsSound, 1);

            bool hasEnabledBool = hasEnabled == 1;

            SetEnabled(hasEnabledBool);
            Debug.Log(hasEnabledBool ? "Music Enabled" : "Music Disabled");

        }
        private void OnValidate()
        {
            SetEnabled(IsMusicEnabled);
            EditorUtility.SetDirty(this);
        }

    }
}
