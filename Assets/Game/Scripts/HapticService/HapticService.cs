using System;
using UnityEngine;
using Zenject;

namespace Scripts.Haptics
{

    public class HapticService : IHapticService
    {

        public HapticConfigs Config { get; private set; }

        private float m_LastHapticTime = 0f;

        [Inject]
        public void Contruct(HapticConfigs config)
        {
            Config = config;
            Config.LoadSettings();
        }

        public void Play(HapticType hapticType)
        {

            if (Time.unscaledTime - m_LastHapticTime < Config.m_CooldownTime) return;

            PlayMobile(hapticType, Config.m_GlobalIntensity);
            m_LastHapticTime = Time.unscaledTime;

        }
        public void SetHapticEnabled(bool isEnabled)
        {
            Config.SetEnabled(isEnabled);
        }


        public static void PlayMobile(HapticType hapticType, float vibreationDensity)
        {
#if UNITY_ANDROID || UNITY_IOS
            switch (hapticType)
            {
                case HapticType.NONE:
                    break;
                case HapticType.LIGHT:
                    Handheld.Vibrate();
                    break;
                case HapticType.MEDIUIM:
                    break;
                case HapticType.HEAVY:
                    break;
                case HapticType.SELECTION:
                    break;
                default:
                    break;
            }

#endif
        }
    }
}
