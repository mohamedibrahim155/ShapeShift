using UnityEditor;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "HapticConfigs", menuName = "Scriptable Objects/Haptics/HapticConfigs")]
public class HapticConfigs : ScriptableObject
{
    [Header("data")]
    public  float m_GlobalIntensity = 1f;
    public float m_CooldownTime = 0.1f;


    public bool isHapticEnabled;

    public event Action<bool> OnHapticEnabledChanged = delegate { };


    public void SetEnabled(bool enabled)
    {
        isHapticEnabled = enabled;
        PlayerPrefs.SetInt(PlayerPrefsKeys.SettingsHaptic, enabled ? 1 : 0);
        PlayerPrefs.Save();
        OnHapticEnabledChanged.Invoke(isHapticEnabled);


    }

    public void LoadSettings()
    {
        int enabledInt = PlayerPrefs.GetInt(PlayerPrefsKeys.SettingsHaptic, 1);
        bool enabled = enabledInt == 1;
        SetEnabled(enabled);

        Debug.Log(enabled ? "Haptic Enabled" : "Haptic Disabled");
    }

    private void OnValidate()
    {
        SetEnabled(isHapticEnabled);
        EditorUtility.SetDirty(this);
    }
}
