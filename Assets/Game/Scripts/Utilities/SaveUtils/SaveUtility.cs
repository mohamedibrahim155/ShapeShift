using UnityEngine;

public static class SaveUtility
{
    private const float SaveInterval = 0.25f;
    private static float _lastSavedTime = 0f;
    public static void SaveData(string key, object value)
    {

        if (value is int intValue)
        {
            PlayerPrefs.SetInt(key, intValue);
        }
        else if (value is float floatValue)
        {
            PlayerPrefs.SetFloat(key, floatValue);
        }
        else if (value is string stringValue)
        {
            PlayerPrefs.SetString(key, stringValue);
        }
        else if (value is bool boolValue)
        {
            PlayerPrefs.SetInt(key, boolValue ? 1 : 0);
        }

        SaveImmediate();
    }

    public static void SaveImmediate()
    {
        PlayerPrefs.Save();
        _lastSavedTime = Time.unscaledTime;
    }

    public static void SaveDebounced(string key, object value)
    {
        if (Time.unscaledTime - _lastSavedTime < SaveInterval) return;
        SaveData(key, value);
    }

    public static object LoadData(string key, object defaultValue)
    {

        if (defaultValue is int)
        {
            return PlayerPrefs.GetInt(key, (int)defaultValue);
        }
        else if (defaultValue is float)
        {
            return PlayerPrefs.GetFloat(key, (float)defaultValue);
        }
        else 
        {
            return PlayerPrefs.GetString(key, defaultValue.ToString());
        }
    }
}
