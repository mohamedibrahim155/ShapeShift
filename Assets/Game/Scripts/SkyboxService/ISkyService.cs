using System;
using UnityEngine;

namespace Scripts.SkyService
{
    public interface ISkyService 
    {
 
        void Initialize();
        void SetSky(ESkyColorType type);
        void LerpToSky(ESkyColorType type, float duration);

        void CleanUp();
    }
}
