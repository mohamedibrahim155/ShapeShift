using System;
using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.STP;

namespace Scripts.SkyService
{
    public class SkyboxView : MonoBehaviour
    {
        private SkyboxConfig config;

        private Material skyboxMaterial;


        public void Initialize(SkyboxConfig config)
        {
            this.config = config;

            SpawnMaterial();
        }

        private void SpawnMaterial()
        {
            skyboxMaterial = UnityEngine.Object.Instantiate(config.SkyBoxMaterial);
            RenderSettings.skybox = skyboxMaterial;
        }

        public void ApplySkyColor(SkyColor color)
        {
            skyboxMaterial.SetColor(SkyboxConfig.GetTopColorString(), color.Top);
            skyboxMaterial.SetColor(SkyboxConfig.GetBottomColorString(), color.Bottom);
        }

    }
}
