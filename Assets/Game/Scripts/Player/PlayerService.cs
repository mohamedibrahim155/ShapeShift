
using Scripts.GameService;
using Scripts.Level;
using Scripts.Score;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Zenject;
namespace Scripts.Player
{

    public class PlayerService : IPlayerService
    {

        private bool _finishLineReached;
        private float _currentWaitTime;

        private PlayerConfig m_PlayerConfig;
        private PlayerView m_PlayerView;
        private DiContainer m_Container;
        private IPLayerInputService m_PlayerInputService;
        private IGameLoopService m_GameloopService;
        private ICameraService m_CameraService;
        private IScoreService m_ScoreService;
        private ILevelService m_LevelService;
        private PlayerStateMachine PlayerStateMachine;



        private List<BlockView> m_ListOfBlocks;
        private Dictionary<ESwipeDirection, ShapeView> m_PlayerShapes =  new Dictionary<ESwipeDirection, ShapeView>();
        private const int PointsPerCorrectCollision = 10;

        [Inject]
        private void Construct(PlayerConfig playerConfig, DiContainer container , IPLayerInputService inputService,
            IGameLoopService gameloopService, ICameraService cameraService, IScoreService scoreService, ILevelService levelService)

        {
            m_PlayerConfig = playerConfig;
            m_Container = container;
            m_PlayerInputService = inputService;
            m_GameloopService = gameloopService;
            m_CameraService = cameraService;
            m_ScoreService = scoreService;
            m_LevelService = levelService;




            OnInitialize();
        }

        void InitializeStateMachine(PlayerView view, PlayerConfig config)
        {
            PlayerStateMachine = new PlayerStateMachine(view, config);

            PlayerStateMachine.AddState(EPlayerStates.IDLE, new IdleState());
            PlayerStateMachine.AddState(EPlayerStates.MOVE, new MoveState());
            PlayerStateMachine.AddState(EPlayerStates.FALL, m_Container.Instantiate<FallState>());
        }

        void OnInitialize()
        {
            _currentWaitTime = 0;
            m_PlayerConfig.m_HasPlayerFinished = false;

            m_GameloopService.OnUpdateTick += Update;
            m_GameloopService.OnFixedUpdateTick += FixedUpdate;
            m_GameloopService.OnGizemosTick += OnGizmosDraw;
            m_LevelService.OnLevelCompleted += OnPlayerReachedFinishLine;

            m_ScoreService.Reset();
            SpawnLevel();
            SpawnPlayer(m_PlayerConfig.m_SpawnPosition);
        }

        // Spawn the player at the specified position and set up input and collision handling
        public void SpawnPlayer(Vector3 position)
        {
            m_PlayerView = m_Container.InstantiatePrefabForComponent<PlayerView>(m_PlayerConfig.m_PlayerView);
            m_PlayerView.SpawnShapes(m_Container);

            m_PlayerView.transform.position = position;

            m_PlayerView.ChangeShape(EShapeType.CUBE);


            InitializeStateMachine(m_PlayerView, m_PlayerConfig);

            InitializeCamera();
            HandleSwipe();
            HandleCollision();

            PlayerStateMachine.ChangeState(EPlayerStates.MOVE);

            m_CameraService.EnableCamera(ECameraType.FOLLOW_CAMERA);
        }

        public void SpawnLevel()
        {
              m_LevelService.CreateLevel(0);
        }


        private void InitializeCamera()
        {
            m_CameraService.SpawnCamera(Vector2.zero);
            m_CameraService.SetCameraFollow(m_PlayerView.transform);
            m_CameraService.SetCameraLookAt(m_PlayerView.transform);
        }

        // Set up collision handling
        private void HandleCollision()
        {
            BlockWallColliderView.OnBlockCollision += OnBlockCollision;
            m_LevelService.OnLevelCompleted += OnPlayerReachedFinishLine;
        }

        // Set up swipe input handling
        private void HandleSwipe()
        {
            m_PlayerInputService.OnSwipe += Swipe;
        }

        // Change the player's shape based on swipe direction
        private void Swipe(ESwipeDirection swipeDirection)
        {
          m_PlayerView.ChangeShapeForDirection(swipeDirection);
        }

        private void OnBlockCollision(BlockView block)
        {
            bool isValid = IsValidCollision(m_PlayerConfig.m_CurrentShapeType, block.BlockType);

            Debug.Log($"Collision: {isValid}, Player: {m_PlayerConfig.m_CurrentShapeType}, Block: {block.BlockType}");
            if (!isValid)
            {
                // Level Failed Condition
                Debug.Log("Player Collided with different Shape. Level Failed!");
                //DestroyPlayer();
                return;
            }

            m_ScoreService.AddPoints(PointsPerCorrectCollision);
        }

        private void Update()
        {
            // update inputs
            m_PlayerInputService.UpdateInputs();

            // update states
            if (PlayerStateMachine!= null)
            {
                PlayerStateMachine.Update();
            }

            CheckFinishPoint();
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

        private void OnPlayerReachedFinishLine()
        {
            m_PlayerConfig.m_HasPlayerFinished = true;

            // change to idle
            PlayerStateMachine.ChangeState(EPlayerStates.IDLE);
            m_CameraService.EnableCamera(ECameraType.FINISHLINE_CAMERA);
            
        }


        private void Reset()
        {
            DestroyPlayer();
            OnInitialize();
        }


        private void DestroyPlayer()
        {
            MonoBehaviour.Destroy(m_PlayerView.gameObject);
            CleanUp();

        }


        public void CheckFinishPoint()
        {
            if (m_PlayerConfig.m_HasPlayerFinished)
            {
                if (_currentWaitTime  > m_PlayerConfig.m_FinishLineWaitTimer)
                {

                    Reset();
                    return;
                }
                _currentWaitTime += Time.deltaTime;
            }
        }
     

        private bool IsValidCollision(EShapeType shapeType, EBlockType blockType)
        {
          
            return (int) shapeType == (int) blockType;
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
            BlockWallColliderView.OnBlockCollision -= OnBlockCollision;
            m_PlayerInputService.OnSwipe -= Swipe;
            m_GameloopService.OnUpdateTick -= Update;
            m_GameloopService.OnFixedUpdateTick -= FixedUpdate;

            m_PlayerInputService.CleanUp();
            m_CameraService.Cleanup();
            m_LevelService.Cleanup();
        }

        private void OnGameStart()
        {
            
        }
    }
}
