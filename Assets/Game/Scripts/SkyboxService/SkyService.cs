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
        public SkyboxView Skyview { get; private set; }
        private SkyColor CurrentSkyColor { get;  set; }
        public ESkyColorType CurrentSkyColorType { get;  private set; }
        private bool IsTransitioning { get; set; }

        private Material skyboxMaterial;
        private ESkyColorType transitionTargetColorWithType;

        private float transitionDuration;
        private float transitionElapsed;
        private bool isInitalized;

        private IGameLoopService m_GameLoopService;
        private IPlayerService m_PlayerService;
        private DiContainer m_Container;

        public event Action OnSkyLerpStarted = delegate { };
        public event Action OnSkyLerpCompleted= delegate { };


        [Inject]
        public void Contruct(SkyboxConfig config, IGameLoopService gameLoopService, IPlayerService playerService, DiContainer container)
        {
            Config = config;
            m_GameLoopService = gameLoopService;
            m_PlayerService = playerService;
            m_Container = container;
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

            //spawns view
            Skyview = m_Container.InstantiatePrefabForComponent<SkyboxView>(Config.viewPrefab);
            Skyview.Initialize(Config);

            
            Reset();


            m_GameLoopService.OnUpdateTick             += Update;
            m_PlayerService.OnPlayerBeastModeActivated += OnBeastModeActivated;
            isInitalized = true;
        }

        private void Update()
        {
            HandleLerpColor(Time.deltaTime);
        }


        private void HandleLerpColor(float deltaTime)
        {
            if (!IsTransitioning)
            {
                return;
            }

            transitionElapsed += deltaTime;

            float t = transitionDuration <= 0 ? 1 : Mathf.Clamp01(transitionElapsed / (float)transitionDuration);

            LerpCurrentTo(transitionTargetColorWithType, t);
            if (t >= 1)
            {
                //IsTransitioning = false;
                //CurrentSkyColorType = transitionTargetColorWithType;
                SetSky(transitionTargetColorWithType);
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

            Skyview.ApplySkyColor(CurrentSkyColor);
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
            transitionTargetColorWithType = type;
            transitionDuration = duration;
            transitionElapsed = 0f;
            IsTransitioning = true;

            OnSkyLerpStarted.Invoke();
        }

        private SkyColor LerpSky(ESkyColorType fromColorType, ESkyColorType toColorType, float time)
        {
            if (Config == null)
            {
                Debug.LogWarning($"Config is null");
                return Config.GetSkyColorOrDefault(ESkyColorType.DEFAULT);
            }

            SkyColor colorA = Config.GetSkyColorOrDefault(fromColorType);
            SkyColor colorB = Config.GetSkyColorOrDefault(toColorType);
          
            return SkyColor.Lerp(colorA, colorB, time);
        }

        public void CleanUp()
        {
            m_GameLoopService.OnUpdateTick             -= Update;
            m_PlayerService.OnPlayerBeastModeActivated -= OnBeastModeActivated;
        }

        public void LerpCurrentTo( ESkyColorType toColorType, float time)

        {
            if (Config == null)
            {
                Debug.LogWarning($"Config is null");
                CurrentSkyColorType = ESkyColorType.DEFAULT;
            }
            CurrentSkyColor = LerpSky(CurrentSkyColorType, toColorType, time);
           Skyview.ApplySkyColor(CurrentSkyColor);

        }
    }
}
