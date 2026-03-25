using System;
using UnityEngine;

namespace Scripts.SkyService
{
    public interface ISkyService 
    {
        SkyboxConfig Config { get; }
        void Initialize();
        void SetSky(ESkyColorType type);
        void LerpToSky(ESkyColorType type, float duration);
        SkyColor LerpSky(ESkyColorType typeA, ESkyColorType typeB, float time);

        void CleanUp();
    }
}
