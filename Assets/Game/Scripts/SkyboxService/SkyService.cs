using Scripts.GameService;
using UnityEngine;
using Zenject;

namespace Scripts.SkyService
{
    public class SkyService : ISkyService
    {
        private SkyboxConfig Config { get; set; }
        private SkyColor CurrentSkyColor { get;  set; }
        private bool IsTransitioning { get; set; }

        private Material skyboxMaterial;
        private SkyColor transitionStartColor;
        private SkyColor transitionTargetColor;
        private float transitionDuration;
        private float transitionElapsed;
        private bool isInitalized;

        private IGameLoopService m_GameLoopService;


        [Inject]
        public void Contruct(SkyboxConfig config, IGameLoopService gameLoopService)
        {
            Config = config;
            m_GameLoopService = gameLoopService;

            Initialize();
        }

        public void Initialize()
        {
            if (isInitalized)
            {
                return;
            }

            // Clone the material so runtime color changes do not mutate the asset itself.
            skyboxMaterial = Object.Instantiate(Config.SkyBoxMaterial);
            RenderSettings.skybox = skyboxMaterial;

            CurrentSkyColor = Config.GetSkyColorOrDefault(ESkyColorType.DEFAULT);
            ApplySkyColor(CurrentSkyColor);


            m_GameLoopService.OnUpdateTick += Update;
            m_GameLoopService.OnDestroyed += CleanUp;
            isInitalized = true;
        }

        private void Update()
        {
            HandleTick(Time.deltaTime);
        }

        private void HandleTick(float deltaTime)
        {
            if (!IsTransitioning || skyboxMaterial == null)
            {
                return;
            }

            transitionElapsed += deltaTime;

            float t = transitionDuration <= 0f
                ? 1f
                : Mathf.Clamp01(transitionElapsed / transitionDuration);

            SkyColor lerpedColor = new SkyColor
            {
                Top = Color.Lerp(transitionStartColor.Top, transitionTargetColor.Top, t),
                Bottom = Color.Lerp(transitionStartColor.Bottom, transitionTargetColor.Bottom, t)
            };

            CurrentSkyColor = lerpedColor;
            ApplySkyColor(CurrentSkyColor);

            if (t >= 1f)
            {
                IsTransitioning = false;
            }
        }

        public void SetSky(ESkyColorType type)
        {

            CurrentSkyColor = Config.GetSkyColorOrDefault(type);
            IsTransitioning = false;
            transitionElapsed = 0f;
            transitionDuration = 0f;

            ApplySkyColor(CurrentSkyColor);
        }

        public void LerpToSky(ESkyColorType type, float duration)
        {
            if (Config == null)
            {
                return;
            }

            if (duration <= 0f)
            {
                SetSky(type);
                return;
            }

            transitionStartColor = CurrentSkyColor;
            transitionTargetColor = Config.GetSkyColorOrDefault(type);
            transitionDuration = duration;
            transitionElapsed = 0f;
            IsTransitioning = true;
        }

        private void ApplySkyColor(SkyColor color)
        {
            skyboxMaterial.SetColor(SkyboxConfig.GetTopColorString(), color.Top);
            skyboxMaterial.SetColor(SkyboxConfig.GetBottomColorString(), color.Bottom);
        }

        public void CleanUp()
        {
            m_GameLoopService.OnUpdateTick -= Update;
            m_GameLoopService.OnDestroyed -= CleanUp;
        }
    }
}
