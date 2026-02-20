
using Scripts.GameService;
using Scripts.Score;
using System;
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
        private PlayerStateMachine PlayerStateMachine;



        private List<BlockView> m_ListOfBlocks;
        private const int PointsPerCorrectCollision = 10;

        [Inject]
        private void Construct(PlayerConfig playerConfig, DiContainer container , IPLayerInputService inputService,
            IGameLoopService gameloopService, ICameraService cameraService, IScoreService scoreService)
        {
            m_PlayerConfig = playerConfig;
            m_Container = container;
            m_PlayerInputService = inputService;
            m_GameloopService = gameloopService;
            m_CameraService = cameraService;
            m_ScoreService = scoreService;


            m_GameloopService.OnUpdateTick += Update;
            m_GameloopService.OnFixedUpdateTick += FixedUpdate;

            m_ScoreService.Reset();
            SpawnPlayer(Vector3.zero);


        }

        void InitializeStateMachine(PlayerView view, PlayerConfig config)
        {
            PlayerStateMachine = new PlayerStateMachine(view, config);

            //PlayerStateMachine.AddState(EPlayerStates.IDLE, new IdleState());

        }

        // Spawn the player at the specified position and set up input and collision handling
        public void SpawnPlayer(Vector3 position)
        {
            m_PlayerView = m_Container.InstantiatePrefabForComponent<PlayerView>(m_PlayerConfig.m_PlayerView);
            m_PlayerView.SpawnShapes(m_Container);
            m_PlayerView.ChangeShape(EShapeType.CUBE);
            InitializeCamera();
            HandleSwipe();
            HandleCollision();
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
        }
        // Set up swipe input handling
        private void HandleSwipe()
        {
            m_PlayerInputService.OnSwipe += Swipe;
        }

        // Change the player's shape based on swipe direction
        private void Swipe(SwipeDirection swipeDirection)
        {
            // index represents circle =1, cyl=2, triangle=3, pentagon=4
            int swapeIndex = (int)swipeDirection;
            m_PlayerView.ChangeShape((EShapeType)swapeIndex);
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
         
        }

        private void FixedUpdate()
        {
           // m_PlayerView.transform.position += Vector3.forward * 10 * Time.deltaTime;
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
            Debug.Log("Registered Block Wall. Total walls: " + m_ListOfBlocks.Count);
        }

        public void CleanUp()
        {
            BlockWallColliderView.OnBlockCollision -= OnBlockCollision;
            m_PlayerInputService.OnSwipe -= Swipe;
            m_GameloopService.OnUpdateTick -= Update;
            m_GameloopService.OnFixedUpdateTick -= FixedUpdate;

            m_PlayerInputService.CleanUp();
        }
    }
}
