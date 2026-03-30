using Scripts.GameService;
using Scripts.Player;
using System;
using UnityEngine;
using Zenject;

namespace Scripts.SkyService
{
    public class SkyService : ISkyService
    {
        public SkyboxConfig Config { get; private set; }
        private SkyColor CurrentSkyColor { get;  set; }
        public ESkyColorType CurrentSkyColorType { get;  private set; }
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

        public event Action OnSkyLerpStarted = delegate { };
        public event Action OnSkyLerpCompleted= delegate { };


        [Inject]
        public void Contruct(SkyboxConfig config, IGameLoopService gameLoopService, IPlayerService playerService)
        {
            Config = config;
            m_GameLoopService = gameLoopService;
            m_PlayerService = playerService;
            Initialize();
        }

        private void OnBeastModeActivated(bool beastState)
        {
            if (beastState)
            {
                OnBeastActive();
            }
            else 
            {
                OnDeActive();
            }
        }

        private void OnBeastActive()
        {
            LerpToSky(ESkyColorType.GREEN, 0.5f);
        }

        private void OnDeActive()
        {
            //LerpCurrentTo(CurrentSkyColorType, ESkyColorType.DEFAULT, 1);
        }

        public void Initialize()
        {
            if (isInitalized)
            {
                return;
            }

            // Clone the material so runtime color changes do not mutate the asset itself.
            skyboxMaterial = UnityEngine.Object.Instantiate(Config.SkyBoxMaterial);
            RenderSettings.skybox = skyboxMaterial;

       
            int randomSkyIndex = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ESkyColorType)).Length);
            SetSky((ESkyColorType)randomSkyIndex);


            m_GameLoopService.OnUpdateTick             += Update;
            m_PlayerService.OnPlayerBeastModeActivated += OnBeastModeActivated;
            isInitalized = true;
        }

        private void Update()
        {
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
                OnSkyLerpCompleted.Invoke();
            }
        }

        public void Reset()
        {
            int randomSkyIndex = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ESkyColorType)).Length);
            SetSky((ESkyColorType)randomSkyIndex);
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

            OnSkyLerpStarted.Invoke();
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
            m_GameLoopService.OnUpdateTick             -= Update;
            m_PlayerService.OnPlayerBeastModeActivated -= OnBeastModeActivated;
        }

        public void LerpCurrentTo(ESkyColorType typeA, ESkyColorType typeB, float time)

        {
            if (Config == null)
            {
                Debug.LogWarning($"Config is null");
                CurrentSkyColor = Config.GetSkyColorOrDefault(ESkyColorType.DEFAULT);
            }


            CurrentSkyColor = LerpSky(typeA, typeB, time);
            ApplySkyColor(CurrentSkyColor);

        }
    }
}
