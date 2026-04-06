using Scripts.SkyService;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkyboxConfig", menuName = "Scriptable Objects/Skybox/SkyboxConfig")]
public class SkyboxConfig : ScriptableObject, ISerializationCallbackReceiver
{
    public Material SkyBoxMaterial;
    public SkyboxView viewPrefab;

    [SerializeField]
    public List<SkyColorWithType> SkyData = new List<SkyColorWithType>();

    private readonly Dictionary<ESkyColorType, SkyColor> skyColors = new();

    private int MaxSkySize => Enum.GetValues(typeof(ESkyColorType)).Length;
    public IReadOnlyDictionary<ESkyColorType, SkyColor> SkyColors => skyColors;

    public static string GetTopColorString()
    {
        return "_TopColor";
    }

    public static string GetBottomColorString()
    {
        return "_BottomColor";
    }

    public bool TryGetSkyColor(ESkyColorType type, out SkyColor color)
    {
        return skyColors.TryGetValue(type, out color);
    }

    public SkyColor GetSkyColorOrDefault(ESkyColorType type)
    {
        return skyColors.TryGetValue(type, out var color)
            ? color
            : GetFallbackColor();
    }

    private void OnEnable()
    {
        RebuildLookup();
    }

    private void OnValidate()
    {
        RebuildLookup();
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        RebuildLookup();
    }

    private void RebuildLookup()
    {
        ClampSkyData();
        skyColors.Clear();

        for (int i = 0; i < SkyData.Count; i++)
        {
            SkyColorWithType entry = SkyData[i];
            skyColors[entry.Type] = entry.Color;
        }
    }

    private SkyColor GetFallbackColor()
    {
        return SkyData.Count > 0 ? SkyData[0].Color : default;
    }

    private void ClampSkyData()
    {
        if (SkyData == null)
        {
            SkyData = new List<SkyColorWithType>();
            return;
        }

        if (SkyData.Count > MaxSkySize)
        {
            SkyData.RemoveRange(MaxSkySize, SkyData.Count - MaxSkySize);
        }

    }
}

[System.Serializable]
public struct SkyColor
{
    public Color Top;
    public Color Bottom;

    public static SkyColor Lerp(SkyColor a, SkyColor b, float t)
    {
        if (t>= 1)
        {
            t = Mathf.Clamp01(t);
        }

        SkyColor skyColorLerp = new SkyColor
        {
            Top = Color.Lerp(a.Top, b.Top, t),
            Bottom = Color.Lerp(a.Bottom, b.Bottom, t)
        };

        return skyColorLerp;
    }
}

public enum ESkyColorType
{
    DEFAULT = 0,
    BLUE = 1,
    GREYSHADE = 2,
    BONUS = 3,
    GREEN = 4,
}

[System.Serializable]
public struct SkyColorWithType
{
    public ESkyColorType Type;
    public SkyColor Color;
}
