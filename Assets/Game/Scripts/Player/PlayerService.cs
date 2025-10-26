
using UnityEngine;
using Zenject;
using Scripts.GameService;
using System;
namespace Scripts.Player
{

    public class PlayerService : IPlayerService
    {

        private PlayerConfig m_PlayerConfig;
        private PlayerView m_PlayerView;
        private DiContainer m_Container;
        private IPLayerInputService m_PlayerInputService;
        private IGameLoopService m_GameloopService;

        [Inject]
        private void Construct(PlayerConfig playerConfig, DiContainer container , IPLayerInputService inputService, IGameLoopService gameloopService)
        {
            m_PlayerConfig = playerConfig;
            m_Container = container;
            m_PlayerInputService = inputService;
            m_GameloopService = gameloopService;


            m_GameloopService.OnUpdateTick += Update;
            m_GameloopService.OnFixedUpdateTick += FixedUpdate;

            SpawnPlayer(Vector3.zero);


        }

        // Spawn the player at the specified position and set up input and collision handling
        public void SpawnPlayer(Vector3 position)
        {
            m_PlayerView = m_Container.InstantiatePrefabForComponent<PlayerView>(m_PlayerConfig.m_PlayerView);

            HandleSwipe();
            HandleCollision();
        }

        // Set up collision handling
        private void HandleCollision()
        {
            PlayerCollisionListener.OnShapeCollision += ObstacleCollision;
        }
        // Set up swipe input handling
        private void HandleSwipe()
        {
            m_PlayerInputService.OnSwipe += ChangeShape;
        }

        // Change the player's shape based on swipe direction
        private void ChangeShape(SwipeDirection swipeDirection)
        { 
            int swapeIndex = (int)swipeDirection;

            m_PlayerView.DisableShape(m_PlayerConfig.m_CurrentShapeIndex);

            m_PlayerView.EnableShape(swapeIndex);

            m_PlayerConfig.m_CurrentShapeIndex = swapeIndex;
        }

        private void ObstacleCollision(ShapeType shape)
        {
            // Logic to handle obstacle collision based on shape

            Debug.Log("Collided with shape: " + shape.ToString());
        }

        private void Update()
        { 
            
        }

        private void FixedUpdate()
        {
        }


        private void DestroyPlayer()
        {
            MonoBehaviour.Destroy(m_PlayerView.gameObject);

            PlayerCollisionListener.OnShapeCollision -= ObstacleCollision;
            m_PlayerInputService.OnSwipe -= ChangeShape;

        }
    }
}
