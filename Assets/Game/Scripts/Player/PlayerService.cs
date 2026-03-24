
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

        private PlayerConfig m_PlayerConfig;
        private PlayerView m_PlayerView;
        private DiContainer m_Container;
        private IPLayerInputService m_PlayerInputService;
        private IGameLoopService m_GameloopService;
        private ICameraService m_CameraService;
        private IScoreService m_ScoreService;
        private ILevelService m_LevelService;
        private ISkyService m_SkyBoxService;
        private IParticleService m_ParticleService;
        private PlayerStateMachine PlayerStateMachine;
        private PlayerInvisibleController PlayerInvisibleController;


        private Dictionary<ESwipeDirection, ShapeView> m_PlayerShapes = new Dictionary<ESwipeDirection, ShapeView>();
        private const int PointsPerCorrectCollision = 10;

        public event Action OnPlayerFinishedLevel = delegate { };
        public event Action OnPlayerDied = delegate { };

        [Inject]
        private void Construct(PlayerConfig playerConfig, DiContainer container, IPLayerInputService inputService,
            IGameLoopService gameloopService, ICameraService cameraService, IScoreService scoreService, 
            ILevelService levelService, IParticleService particleService, ISkyService skyBoxService)

        {
            m_PlayerConfig = playerConfig;
            m_Container = container;
            m_PlayerInputService = inputService;
            m_GameloopService = gameloopService;
            m_CameraService = cameraService;
            m_ScoreService = scoreService;
            m_LevelService = levelService;
            m_ParticleService = particleService;
            m_SkyBoxService = skyBoxService;


            Initialize();
        }

        private void Initialize()
        {
            //resets player state for new game or level retry
            m_PlayerConfig.m_HasPlayerFinished = false;

            //subscribes events
            InitializeEvents();
            SpawnPlayer(m_PlayerConfig.m_SpawnPosition);
        }

        private void InitializeEvents()
        {
            m_GameloopService.OnUpdateTick += Update;
            m_GameloopService.OnFixedUpdateTick += FixedUpdate;
            m_GameloopService.OnGizemosTick += OnGizmosDraw;
            m_LevelService.OnLevelCompleted += HandlePlayerReachedFinishLine;
            m_PlayerInputService.OnSwipe += Swipe;
            BlockWallColliderView.OnBlockCollision += HandleBlockCollision;

            m_ScoreService.Reset();
        }


        private void InitializeStateMachine()
        {
            PlayerStateMachine = new PlayerStateMachine(m_PlayerView, m_PlayerConfig);

            PlayerStateMachine.AddState(EPlayerStates.IDLE, m_Container.Instantiate<IdleState>());
            PlayerStateMachine.AddState(EPlayerStates.MOVE, new MoveState());
            PlayerStateMachine.AddState(EPlayerStates.FALL, m_Container.Instantiate<FallState>());

            PlayerStateMachine.ChangeState(EPlayerStates.IDLE);

        }



        // Spawn the player at the specified position and set up input and collision handling
        public void SpawnPlayer(Vector3 position)
        {
            m_PlayerView = m_Container.InstantiatePrefabForComponent<PlayerView>(m_PlayerConfig.m_PlayerView);
            m_PlayerView.Initialize(m_PlayerConfig, position);


            InitializeStateMachine();
            InitializeCamera(Vector3.zero);
            Init();

        }

        private void Init()
        {
            PlayerInvisibleController = new PlayerInvisibleController(m_PlayerView, m_PlayerConfig, m_LevelService);

            m_PlayerView.EnableShape(EShapeType.CUBE);
            PlayerStateMachine.GetCurrentState().OnEnterState();
        }


        public void InitializeCamera(Vector3 spawnPosition)
        {
            m_CameraService.SpawnCamera(spawnPosition);
            m_CameraService.SetCameraFollow(m_PlayerView.transform);
            m_CameraService.SetCameraLookAt(m_PlayerView.transform);
        }

        public void StartGame()
        {
            m_PlayerConfig.SetCurrentShape(EShapeType.CUBE);

            //spawns Input
            PlayerStateMachine.ChangeState(EPlayerStates.MOVE);

            m_CameraService.EnableCamera(ECameraType.FOLLOW_CAMERA);

            m_PlayerInputService.SpawnInputController();

        }


        // Change the player's shape based on swipe direction
        private void Swipe(ESwipeDirection swipeDirection)
        {
            m_PlayerView.ChangeShapeForDirection(swipeDirection);
        }

        private void HandleBlockCollision(BlockView block)
        {
            bool isValid = IsValidCollision(m_PlayerConfig.m_CurrentShapeType, block.BlockType);

            if (!isValid)
            {
                // Level Failed Condition
                Debug.Log("Player Collided with different Shape. Level Failed!");

                InvokePlayerDeath();
                return;
            }

            PlayerInvisibleController.NextBlock();

            m_ScoreService.AddPoints(PointsPerCorrectCollision);
        }

        private void Update()
        {
            // update inputs
            m_PlayerInputService.UpdateInputs();

            // update states
            if (PlayerStateMachine != null)
            {
                PlayerStateMachine.Update();

                if (PlayerStateMachine.CurrentStateID == EPlayerStates.MOVE)
                    PlayerInvisibleController.Update();
                else
                    PlayerInvisibleController.Hide();
            }

        }


        private void FixedUpdate()
        {

            if (PlayerStateMachine != null)
            {
                PlayerStateMachine.FixedUpdate();
            }

        }

        private void OnGizmosDraw()
        {
            if (PlayerStateMachine != null)
            {
                PlayerStateMachine.DrawGizmos();
            }
        }
        private void HandlePlayerReachedFinishLine()
        {
            m_PlayerConfig.m_HasPlayerFinished = true;

            // change to idle
            PlayerStateMachine.ChangeState(EPlayerStates.IDLE);
            m_CameraService.EnableCamera(ECameraType.FINISHLINE_CAMERA);
            m_PlayerInputService.DestroyInputController();

            //Activate rotating camera around player at finish line
            m_CameraService.ActivateFinishLineCamera();

            PlayFX(EParticleType.CONFETTI);

            OnPlayerFinishedLevel?.Invoke();
        }

        private void PlayFX(EParticleType type)
        {
            Vector3 spawnPoint = m_PlayerView.transform.position;
            m_ParticleService.SpawnParticle(type, spawnPoint, Quaternion.identity);

        }
        public void Reset()
        {
            DestroyPlayer();
            CleanUp();
            Initialize();
        }

        private void DestroyPlayer()
        {
            MonoBehaviour.Destroy(m_PlayerView.gameObject);
        }


        private bool IsValidCollision(EShapeType shapeType, EBlockType blockType)
        {

            return (int)shapeType == (int)blockType;
        }

        public void CleanUp()
        {
            //Event clean
            m_GameloopService.OnUpdateTick -= Update;
            m_GameloopService.OnFixedUpdateTick -= FixedUpdate;
            m_LevelService.OnLevelCompleted -= HandlePlayerReachedFinishLine;
            BlockWallColliderView.OnBlockCollision -= HandleBlockCollision;
            m_PlayerInputService.OnSwipe -= Swipe;

            //services clean
            m_PlayerInputService.CleanUp();
            m_CameraService.Cleanup();
            PlayerStateMachine.CleanUp();

            //Data/References clean
            PlayerInvisibleController = null;
        }

        public void InvokePlayerDeath()
        {
            PlayFX(EParticleType.DEATH);


            FailedLevel();
            m_PlayerView.Hide();
            OnPlayerDied.Invoke();
        }

        private void FailedLevel()
        {
            PlayerStateMachine.ChangeState(EPlayerStates.IDLE);
            m_PlayerInputService.EnableInput(false);
            m_LevelService.InvokeLevelFailed();
        }

        public float GetPlayerProgressedDistance()
        {
            if (m_PlayerView == null || m_LevelService.FinishLineView == null)
                return 0f;

            return Vector3.Distance(m_PlayerView.transform.position, m_LevelService.FinishLineView.transform.position);

        }

        public float GetTotalProgressedDistance()
        {
            if (m_LevelService.FinishLineView == null)
                return 0f;
            return Vector3.Distance(m_PlayerConfig.m_SpawnPosition, m_LevelService.FinishLineView.transform.position);
        }
    }
}
