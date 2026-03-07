
using Scripts.GameService;
using Scripts.Level;
using Scripts.Score;
using Scripts.UI;
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

        private PlayerConfig m_PlayerConfig;
        private PlayerView m_PlayerView;
        private DiContainer m_Container;
        private IPLayerInputService m_PlayerInputService;
        private IGameLoopService m_GameloopService;
        private ICameraService m_CameraService;
        private IScoreService m_ScoreService;
        private ILevelService m_LevelService;
        private IUiService m_UIService;
        private PlayerStateMachine PlayerStateMachine;



        private List<BlockView> m_ListOfBlocks;
        private Dictionary<ESwipeDirection, ShapeView> m_PlayerShapes =  new Dictionary<ESwipeDirection, ShapeView>();
        private const int PointsPerCorrectCollision = 10;

        [Inject]
        private void Construct(PlayerConfig playerConfig, DiContainer container , IPLayerInputService inputService,
            IGameLoopService gameloopService, ICameraService cameraService, IScoreService scoreService, ILevelService levelService, IUiService uiService)

        {
            m_PlayerConfig = playerConfig;
            m_Container = container;
            m_PlayerInputService = inputService;
            m_GameloopService = gameloopService;
            m_CameraService = cameraService;
            m_ScoreService = scoreService;
            m_LevelService = levelService;
            m_UIService = uiService;




            OnInitialize();
        }

        private void OnInitialize()
        {
            //resets player state for new game or level retry
            m_PlayerConfig.m_HasPlayerFinished = false;

            //subscribes events
            m_GameloopService.OnUpdateTick += Update;
            m_GameloopService.OnFixedUpdateTick += FixedUpdate;
            m_GameloopService.OnGizemosTick += OnGizmosDraw;
            m_LevelService.OnLevelCompleted +=  _ => OnPlayerReachedFinishLine();

            m_ScoreService.Reset();

            SpawnLevel();
            SpawnPlayer(m_PlayerConfig.m_SpawnPosition);

            //spaen camera after player is spawned to ensure it has a reference
            SpawnCamera(Vector3.zero);

        }
        public void StartGame()
        {
            m_UIService.CloseWindow(EWindowID.MinMenu);

            InitializeStateMachine(m_PlayerView, m_PlayerConfig);

            m_PlayerInputService.EnableInput(true);

            HandleSwipe();
            HandleCollision();


            PlayerStateMachine.ChangeState(EPlayerStates.MOVE);

            m_CameraService.EnableCamera(ECameraType.FOLLOW_CAMERA);
        }
        private void InitializeStateMachine(PlayerView view, PlayerConfig config)
        {
            PlayerStateMachine = new PlayerStateMachine(view, config);

            PlayerStateMachine.AddState(EPlayerStates.IDLE, m_Container.Instantiate<IdleState>());
            PlayerStateMachine.AddState(EPlayerStates.MOVE, new MoveState());
            PlayerStateMachine.AddState(EPlayerStates.FALL, m_Container.Instantiate<FallState>());
        }

       

        // Spawn the player at the specified position and set up input and collision handling
        public void SpawnPlayer(Vector3 position)
        {
            m_PlayerView = m_Container.InstantiatePrefabForComponent<PlayerView>(m_PlayerConfig.m_PlayerView);
            m_PlayerView.SpawnShapes(m_Container);
            m_PlayerView.transform.position = position;

            m_PlayerView.ChangeShape(EShapeType.CUBE);
        }

        public void SpawnLevel()
        {
            int currentLevel = m_LevelService.GetCurrentLevel();

            m_LevelService.SpawnLevel(currentLevel);
        }


        public void SpawnCamera(Vector3 spawnPosition)
        {
            m_CameraService.SpawnCamera(spawnPosition);
            m_CameraService.SetCameraFollow(m_PlayerView.transform);
            m_CameraService.SetCameraLookAt(m_PlayerView.transform);
        }

        // Set up collision handling
        private void HandleCollision()
        {
            BlockWallColliderView.OnBlockCollision += OnBlockCollision;
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
            m_PlayerInputService.EnableInput(false);

            // show win screen
            m_UIService.GetWindow(EWindowID.LevelCompleted).Open(1);

            //Activate rotating camera around player at finish line
            m_CameraService.ActivateFinishLineCamera(m_PlayerView.transform);
        }


        public void Reset()
        {
            DestroyPlayer();
            OnInitialize();

            m_UIService.OpenWindow(EWindowID.MinMenu);
        }


        private void DestroyPlayer()
        {
            MonoBehaviour.Destroy(m_PlayerView.gameObject);
            CleanUp();

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
    }
}
