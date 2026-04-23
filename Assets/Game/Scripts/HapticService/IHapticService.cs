using UnityEngine;

namespace Scripts.Haptics
{
    public interface IHapticService
    {
        HapticConfigs Config { get; }

        void Play(HapticType hapticType);
        void SetHapticEnabled(bool isEnabled);
    }
}
