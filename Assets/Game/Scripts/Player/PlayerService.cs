
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

        // Spawn the player at the specified position and set up input and collision handling
        public void SpawnPlayer(Vector3 position)
        {
            m_PlayerView = m_Container.InstantiatePrefabForComponent<PlayerView>(m_PlayerConfig.m_PlayerView);
            SpawnShapes(m_PlayerView);
            m_PlayerView.ChangeShape(EShapeType.CUBE);
            InitializeCamera();
            HandleSwipe();
            HandleCollision();
        }

        public void SpawnShapes(PlayerView playerView)
        {
            playerView.m_ShapeParent = new GameObject("ShapeParent").transform;
            playerView.m_ShapeParent.SetParent(playerView.transform);

            playerView.shapeTransforms = null;
            playerView.shapeTransforms = new GameObject[m_PlayerConfig.m_ListOfShapes.Count];

            foreach (ShapeConfig item in m_PlayerConfig.m_ListOfShapes)
            {
                GameObject shapeInstance = m_Container.InstantiatePrefab(item.m_ShapeView);
                shapeInstance.transform.SetParent(playerView.m_ShapeParent);
                shapeInstance.SetActive(false);
                playerView.shapeTransforms[(int)item.m_ShapeType] = shapeInstance;
            }
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
            PlayerCollisionListener.OnShapeCollision += ObstacleCollision;
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

        private void ObstacleCollision(EShapeType shape, GameObject collisionObject)
        {
            bool isValid = CheckCollision(shape, collisionObject);

            if (!isValid)
            {
                // Level Failed Condition
                Debug.Log("Player Collided with different Shape. Level Failed!");
                DestroyPlayer();
                return;
            }

            m_ScoreService.AddPoints(PointsPerCorrectCollision);
        }

        private void Update()
        { 
         
        }

        private void FixedUpdate()
        {
            //m_PlayerView.transform.position += Vector3.forward * 10 * Time.deltaTime;
        }


        private void DestroyPlayer()
        {
            MonoBehaviour.Destroy(m_PlayerView.gameObject);

            PlayerCollisionListener.OnShapeCollision -= ObstacleCollision;
            m_PlayerInputService.OnSwipe -= Swipe;

        }

        private bool CheckCollision(EShapeType shapoType, GameObject collisionObject)
        {

            if (m_ListOfBlocks == null)
            {
                Debug.LogWarning("No registered blocks for collision checking.");
                return false;
            }

            if (collisionObject == null)
            {
                return false;
            }

            // Implement collision checking logic here

            foreach (BlockView item in m_ListOfBlocks)
            {
                // finds object that collides
                if (item.gameObject == collisionObject)
                {
                    // checks if the shape type matches the block type

                    return (IsValidCollision(shapoType, item.BlockType));
                }
            }

            return false;
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
    }
}
