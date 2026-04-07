
using Scripts.GameplayStates;
using Scripts.GameService;
using Scripts.Level;
using Scripts.Particle;
using Scripts.Score;
using Scripts.SkyService;
using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Zenject;
using Zenject.SpaceFighter;
namespace Scripts.Player
{

    public class PlayerService : IPlayerService
    {
        
        public PlayerConfig PlayerConfig { get; private set; }
        private PlayerView m_PlayerView;
        private DiContainer m_Container;
        private IPLayerInputService m_PlayerInputService;
        private IGameLoopService m_GameloopService;
        private ICameraService m_CameraService;
        private IScoreService m_ScoreService;
        private ILevelService m_LevelService;
        private IParticleService m_ParticleService;
        private ISkyService m_SkyboxService;
        private PlayerController m_PlayerController;

        private Dictionary<ESwipeDirection, ShapeView> m_PlayerShapes = new Dictionary<ESwipeDirection, ShapeView>();
        private const int PerfectCollsionPoints = 1;

        public event Action OnPlayerFinishedLevel = delegate { };
        public event Action OnPlayerDied = delegate { };
        public event Action<BlockView> OnPlayerCrossedWall = delegate { };
        public event Action<BlockView> OnPlayerCollidedWithWall = delegate { };
        public event Action<bool> OnPlayerBeastModeActivated = delegate { };

        [Inject]
        private void Construct(PlayerConfig playerConfig, DiContainer container, IPLayerInputService inputService,
            IGameLoopService gameloopService, ICameraService cameraService, IScoreService scoreService, 
            ILevelService levelService, IParticleService particleService, ISkyService skyBoxService)

        {
            PlayerConfig = playerConfig;
            m_Container = container;
            m_PlayerInputService = inputService;
            m_GameloopService = gameloopService;
            m_CameraService = cameraService;
            m_ScoreService = scoreService;
            m_LevelService = levelService;
            m_ParticleService = particleService;
            m_SkyboxService = skyBoxService;

            Initialize();
        }

        private void Initialize()
        {


            //subscribes events
            InitializeEvents();

            SpawnPlayer(PlayerConfig.m_SpawnPosition);
        }

        private void InitializeEvents()
        {
            m_GameloopService.OnUpdateTick      += Update;
            m_GameloopService.OnFixedUpdateTick += FixedUpdate;
            m_GameloopService.OnGizemosTick     += OnGizmosDraw;
           
            m_PlayerInputService.OnSwipe        += HandleSwipe;
            m_LevelService.OnLevelCompleted     += HandlePlayerReachedFinishLine;
        }


   
  



        // Spawn the player at the specified position and set up input and collision handling
        public void SpawnPlayer(Vector3 position)
        {
            m_PlayerView = m_Container.InstantiatePrefabForComponent<PlayerView>(PlayerConfig.m_PlayerView, position, Quaternion.identity, null);
            m_PlayerView.Initialize(PlayerConfig);

            m_PlayerController = new PlayerController(PlayerConfig, m_PlayerView, m_LevelService, m_CameraService, m_ParticleService, m_ScoreService, m_SkyboxService);

            BindControllerEvents();
            InitializeCamera(Vector3.zero);

        }


        private void BindControllerEvents()
        {
            m_PlayerController.OnDied               += HandleControllerDied;
            m_PlayerController.OnFinishedLevel      += HandleControllerFinishedLevel;
            m_PlayerController.OnCrossedWall        += HandleControllerCrossedWall;
        }


        private void UnbindControllerEvents()
        {
            if (m_PlayerController == null)
                return;

            m_PlayerController.OnDied                 -= HandleControllerDied;
            m_PlayerController.OnFinishedLevel        -= HandleControllerFinishedLevel;
            m_PlayerController.OnCrossedWall          -= HandleControllerCrossedWall;
        }


        public void InitializeCamera(Vector3 spawnPosition)
        {
            m_CameraService.SpawnCamera(spawnPosition);
            m_CameraService.SetCameraFollow(m_PlayerController.Transform);
            m_CameraService.SetCameraLookAt(m_PlayerController.Transform);
        }

        public void StartGame()
        {
            m_PlayerController.StartGame();
            m_CameraService.EnableCamera(ECameraType.FOLLOW_CAMERA);
            m_PlayerInputService.SpawnInputController();
        }


        // Change the player's shape based on swipe direction
        private void HandleSwipe(ESwipeDirection swipeDirection)
        {
            m_PlayerController.HandleSwipe(swipeDirection);
        }

        public void CheckCollision(BlockView block)
        {
            m_PlayerController.CheckCollision(block);
        }

        private void Update()
        {
            // update inputs
            m_PlayerInputService.UpdateInputs();

            // update states
            m_PlayerController.Tick();

        }


        private void FixedUpdate()
        {
            m_PlayerController.FixedTick();
        }

        private void OnGizmosDraw()
        {
            m_PlayerController.DrawGizmos();
        }
        private void HandlePlayerReachedFinishLine()
        {
            m_PlayerInputService.DestroyInputController();
            m_CameraService.EnableCamera(ECameraType.FINISHLINE_CAMERA);
            m_CameraService.ActivateFinishLineCamera();

            //Activate rotating camera around player at finish line
            m_PlayerController.ReachFinishLine();
        }
        private void HandleControllerDied()
        {
            m_PlayerInputService.EnableInput(false);
            m_LevelService.InvokeLevelFailed();
            OnPlayerDied.Invoke();

        }

        private void HandleControllerFinishedLevel()
        {
            OnPlayerFinishedLevel.Invoke();
        }

        private void HandleControllerCrossedWall(BlockView block)
        {
            OnPlayerCrossedWall.Invoke(block);
            HandleSuccessfullCollision();
        }

        private void HandleSuccessfullCollision()
        {
            const int PointsPerWall = 1;
            const float scoreDivident = 5f;

            bool activateBeastMode = ((m_ScoreService.CurrentScore + PointsPerWall) % scoreDivident == 0) && m_ScoreService.CurrentScore > 0;
            if (activateBeastMode)
            {
                OnPlayerBeastModeActivated.Invoke(true);
                return;
            }

            float value = Mathf.Clamp01((int)m_ScoreService.CurrentScore / scoreDivident);
            m_SkyboxService.LerpCurrentTo(ESkyColorType.GREYSHADE, value);
            m_ScoreService.AddPoints(PointsPerWall);
        }
    

        //resets the player to initial state for new game or level retry
        public void Reset()
        {
            DestroyPlayer();
            CleanUpRuntimeOnly();

            //reset services
            m_ScoreService.Reset();
            m_SkyboxService.Reset();

            // resspawn player and reinitialize states and events
            SpawnPlayer(PlayerConfig.m_SpawnPosition);
        }

        private void DestroyPlayer()
        {
            if (m_PlayerView!= null)
            {
                UnityEngine.Object.Destroy(m_PlayerView.gameObject);
            }
        }

        public void CleanUp()
        {
            CleanUpRuntimeOnly();
            //Event clean
            m_GameloopService.OnUpdateTick -= Update;
            m_GameloopService.OnFixedUpdateTick -= FixedUpdate;
            m_LevelService.OnLevelCompleted -= HandlePlayerReachedFinishLine;
            m_PlayerInputService.OnSwipe -= HandleSwipe;

            //services clean
            m_PlayerInputService.CleanUp();
            m_CameraService.Cleanup();
            m_SkyboxService.CleanUp();
        }

        public void InvokePlayerDeath()
        {
            FailedLevel();
            OnPlayerDied.Invoke();
        }

        private void FailedLevel()
        {
            m_PlayerInputService.EnableInput(false);
            m_LevelService.InvokeLevelFailed();
        }

        public float GetPlayerDistanceToFinish()
        {
            if (m_PlayerController == null || m_LevelService.m_FinishLineView == null)
                return 0f;

            return m_PlayerController.GetDistanceSq(m_LevelService.m_FinishLineView.transform);

        }

        private void CleanUpRuntimeOnly()
        {
            UnbindControllerEvents();

            m_PlayerController?.Cleanup();
            m_PlayerController = null;
            m_PlayerView = null;
        }
    }
}
