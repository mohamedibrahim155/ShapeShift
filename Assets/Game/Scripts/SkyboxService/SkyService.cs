using Scripts.GameService;
using Scripts.Player;
using UnityEngine;
using Zenject;

namespace Scripts.SkyService
{
    public class SkyService : ISkyService
    {
        public SkyboxConfig Config { get; private set; }
        private SkyColor CurrentSkyColor { get;  set; }
        private ESkyColorType CurrentSkyColorType { get;  set; }
        private bool IsTransitioning { get; set; }

        private Material skyboxMaterial;
        private SkyColor transitionStartColor;
        private SkyColor transitionTargetColor;

        private ESkyColorType transitionStartColorWithType;
        private ESkyColorType transitionTargetColorWithType;

        private float transitionDuration;
        private float transitionElapsed;
        private bool isInitalized;

        private IGameLoopService m_GameLoopService;
        private IPlayerService m_PlayerService;


        [Inject]
        public void Contruct(SkyboxConfig config, IGameLoopService gameLoopService, IPlayerService playerService)
        {
            Config = config;
            m_GameLoopService = gameLoopService;
            m_PlayerService = playerService;

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

         //   CurrentSkyColor = Config.GetSkyColorOrDefault(ESkyColorType.DEFAULT);

            SetSky(ESkyColorType.DEFAULT);
           // ApplySkyColor(CurrentSkyColor);


            m_GameLoopService.OnUpdateTick += Update;
            m_GameLoopService.OnDestroyed += CleanUp;
            isInitalized = true;
        }

        private void Update()
        {
            //HandleTick(Time.deltaTime);
            HandleLerpColor(Time.deltaTime);
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

        private void HandleLerpTick(float deltaTime)
        {
            if (!IsTransitioning)
            {
                return;
            }

            transitionElapsed += deltaTime;

            float t = transitionDuration <= 0 ? 1 : Mathf.Clamp01(transitionElapsed / (float)transitionDuration);

            CurrentSkyColor = SkyColor.Lerp(transitionStartColor, transitionTargetColor, t);
            ApplySkyColor(CurrentSkyColor);
            if (t >=1)
            {
                IsTransitioning = false;
            }
        }

        private void HandleLerpColor(float deltaTime)
        {
            if (!IsTransitioning)
            {
                return;
            }

            transitionElapsed += deltaTime;

            float t = transitionDuration <= 0 ? 1 : Mathf.Clamp01(transitionElapsed / (float)transitionDuration);

            CurrentSkyColor = LerpSky(transitionStartColorWithType, transitionTargetColorWithType, t);
            ApplySkyColor(CurrentSkyColor);
            if (t >= 1)
            {
                IsTransitioning = false;
            }
        }

        private void Reset()
        {
            SetSky(ESkyColorType.DEFAULT);
        }

        public void SetSky(ESkyColorType type)
        {

            CurrentSkyColor = Config.GetSkyColorOrDefault(type);
            CurrentSkyColorType = type;
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
            transitionStartColorWithType =CurrentSkyColorType;
            transitionTargetColor = Config.GetSkyColorOrDefault(type);
            transitionTargetColorWithType = type;
            transitionDuration = duration;
            transitionElapsed = 0f;
            IsTransitioning = true;
        }

        public SkyColor LerpSky(ESkyColorType typeA, ESkyColorType typeB, float time)
        {
            if (Config == null)
            {
                Debug.LogWarning($"Config is null");
                return Config.GetSkyColorOrDefault(ESkyColorType.DEFAULT);
            }

            SkyColor colorA = Config.GetSkyColorOrDefault(typeA);
            SkyColor colorB = Config.GetSkyColorOrDefault(typeB);
          
            return SkyColor.Lerp(colorA, colorB, time);
        }

        public void ApplySkyColor(SkyColor color)
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
