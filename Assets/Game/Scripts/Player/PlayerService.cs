
using Scripts.GameService;
using Scripts.Level;
using Scripts.Particle;
using Scripts.Score;
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
        private IParticleService m_ParticleService;
        private PlayerStateMachine PlayerStateMachine;

        [Header("Blocks")]
        private int _currentBlockIndex = 0;
        private List<BlockView> m_ListOfBlocks;

        private Dictionary<ESwipeDirection, ShapeView> m_PlayerShapes = new Dictionary<ESwipeDirection, ShapeView>();
        private const int PointsPerCorrectCollision = 10;

        public event Action OnPlayerFinishedLevel = delegate { };
        public event Action OnPlayerDied = delegate { };

        [Inject]
        private void Construct(PlayerConfig playerConfig, DiContainer container, IPLayerInputService inputService,
            IGameLoopService gameloopService, ICameraService cameraService, IScoreService scoreService, 
            ILevelService levelService, IParticleService particleService)

        {
            m_PlayerConfig = playerConfig;
            m_Container = container;
            m_PlayerInputService = inputService;
            m_GameloopService = gameloopService;
            m_CameraService = cameraService;
            m_ScoreService = scoreService;
            m_LevelService = levelService;
            m_ParticleService = particleService;


            Initialize();
        }

        private void Initialize()
        {
            //resets player state for new game or level retry
            m_PlayerConfig.m_HasPlayerFinished = false;

            //subscribes events
            InitializeEvents();
            InitlializeLevelBlocks();
            SpawnPlayer(m_PlayerConfig.m_SpawnPosition);
        }

        private void InitializeEvents()
        {
            m_GameloopService.OnUpdateTick += Update;
            m_GameloopService.OnFixedUpdateTick += FixedUpdate;
            m_GameloopService.OnGizemosTick += OnGizmosDraw;
            m_LevelService.OnLevelCompleted += OnPlayerReachedFinishLine;
            m_PlayerInputService.OnSwipe += Swipe;
            BlockWallColliderView.OnBlockCollision += OnBlockCollision;

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
            _currentBlockIndex = 0;

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
            //spawns Input
            m_PlayerInputService.SpawnInputController();
            PlayerStateMachine.ChangeState(EPlayerStates.MOVE);

            m_CameraService.EnableCamera(ECameraType.FOLLOW_CAMERA);
        }


        // Change the player's shape based on swipe direction
        private void Swipe(ESwipeDirection swipeDirection)
        {
            m_PlayerView.ChangeShapeForDirection(swipeDirection);
        }

        private void OnBlockCollision(BlockView block)
        {
            bool isValid = IsValidCollision(m_PlayerConfig.m_CurrentShapeType, block.BlockType);

            if (!isValid)
            {
                // Level Failed Condition
                Debug.Log("Player Collided with different Shape. Level Failed!");

                InvokePlayerDeath();
                return;
            }

            _currentBlockIndex++;

            m_PlayerView.HideTransparentShapes();

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

              //  UpdateShapeHighligher();
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

        private void UpdateShapeHighligher()
        {
            BlockView view = GetClosestBlock();

       

            if (view == null) return;

            float distance = GetDistanceFromBlock(view);


            if (_currentBlockIndex >= m_ListOfBlocks.Count)
            {
                Debug.Log("Surpassed value");
                m_PlayerView.UpdateHighligherPosition(Vector3.zero, Color.red, m_PlayerConfig.m_CurrentShapeType);
                return;

            }
            if (distance < 1000)
            {
                Color alpha = new Color(1, 1, 1, 0.5f);
                Color highlightColor = (IsValidCollision(m_PlayerConfig.m_CurrentShapeType, view.BlockType) ? Color.green : Color.red) * alpha;
                m_PlayerView.UpdateHighligherPosition(view.transform.position, highlightColor, m_PlayerConfig.m_CurrentShapeType);
            }
        }

        private void InitlializeLevelBlocks()
        {
            Debug.Log("InitalizeBlocksList");

            var currentBlocks =  m_LevelService.GetCurrentLevelBlocks();
            if (currentBlocks ==  null || currentBlocks.Count == 0)
            {
                Debug.Log("currentBlocks null");
            }
            foreach (var blockView in currentBlocks)
            {
                RegisterBlockWall(blockView);
            }
        }

        private void OnPlayerReachedFinishLine()
        {
            m_PlayerConfig.m_HasPlayerFinished = true;

            // change to idle
            PlayerStateMachine.ChangeState(EPlayerStates.IDLE);
            m_CameraService.EnableCamera(ECameraType.FINISHLINE_CAMERA);
            m_PlayerInputService.DestroyInputController();

            //Activate rotating camera around player at finish line
            m_CameraService.ActivateFinishLineCamera(m_PlayerView.transform);

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

        private BlockView GetClosestBlock()
        {
            if(m_PlayerView == null || m_ListOfBlocks == null) return null;

            int currentIndex = _currentBlockIndex;

            if (currentIndex < m_ListOfBlocks.Count)
            {
                return m_ListOfBlocks[currentIndex];

            }

            return null;

        }

        private float GetDistanceFromBlock(BlockView blockView)
        {
            Vector3 displacement= m_PlayerView.transform.position - blockView.transform.position;
            float distanceSqXZ = displacement.x * displacement.x + displacement.z * displacement.z;

            return distanceSqXZ;
        }


        private void DestroyPlayer()
        {
            MonoBehaviour.Destroy(m_PlayerView.gameObject);
        }


        private bool IsValidCollision(EShapeType shapeType, EBlockType blockType)
        {

            return (int)shapeType == (int)blockType;
        }

        public void RegisterBlockWall(BlockView wall)
        {
            if (m_ListOfBlocks == null)
            {
                m_ListOfBlocks = new List<BlockView>();
            }
            m_ListOfBlocks.Add(wall);
        }

        public void CleanUp()
        {
            m_GameloopService.OnUpdateTick -= Update;
            m_GameloopService.OnFixedUpdateTick -= FixedUpdate;

            m_LevelService.OnLevelCompleted -= OnPlayerReachedFinishLine;

            BlockWallColliderView.OnBlockCollision -= OnBlockCollision;
            m_PlayerInputService.OnSwipe -= Swipe;

            m_PlayerInputService.CleanUp();
            m_CameraService.Cleanup();

            PlayerStateMachine.CleanUp();

            m_ListOfBlocks.Clear();
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
