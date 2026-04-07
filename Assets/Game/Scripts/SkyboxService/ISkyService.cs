using System;
using UnityEngine;

namespace Scripts.SkyService
{
    public interface ISkyService 
    {
        SkyboxConfig Config { get; }
        SkyboxView Skyview { get; }
        event Action OnSkyLerpStarted;
        event Action OnSkyLerpCompleted;
         ESkyColorType CurrentSkyColorType { get; }
        void Initialize();
        void SetSky(ESkyColorType type);
        void LerpToSky(ESkyColorType type, float duration);
        void LerpCurrentTo( ESkyColorType typeB, float time);

        void Reset();
        void CleanUp();
    }
}
