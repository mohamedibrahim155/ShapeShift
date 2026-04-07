using Scripts.Level;
using Scripts.Particle;
using Scripts.Score;
using Scripts.SkyService;
using System;
using UnityEngine;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;
namespace Scripts.Player
{
    public class PlayerController
    {
        private readonly PlayerConfig _config;
        private readonly PlayerView _view;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerInvisibleController _playerInvisibleController;
        private readonly ILevelService _levelService;
        private readonly ICameraService _cameraService;
        private readonly IParticleService _particleService;


        public event Action OnDied = delegate { };
        public event Action OnFinishedLevel = delegate { };
        public event Action<BlockView> OnCrossedWall = delegate { };

        public EShapeType CurrentShapeType { get; private set; }
        public bool HasFinishedLevel { get; private set; }
        public bool HasDied { get; private set; }
        public Transform Transform => _view.transform;
        public Rigidbody Rigidbody => _view.Rigidbody;
        public PlayerController(PlayerConfig config,
            PlayerView view, ILevelService
            levelService,
            ICameraService cameraService,
            IParticleService particleService, 
            IScoreService scoreService, 
            ISkyService skyService)
        {
            //Dependency Injection
            _config = config;
            _view = view;
            _cameraService = cameraService;
            _levelService = levelService;
            _particleService = particleService;


            _view.Initialize(_config);

            //Creation
            _stateMachine = new PlayerStateMachine(_view, _config);
            _playerInvisibleController = new PlayerInvisibleController(this, _config, levelService);

            //states initalize
            _stateMachine.AddState(EPlayerStates.IDLE, new IdleState());
            _stateMachine.AddState(EPlayerStates.MOVE, new MoveState(_playerInvisibleController, this));
            _stateMachine.AddState(EPlayerStates.FALL, new FallState(_cameraService,this));

            SetShapeImmediate(EShapeType.CUBE);
            _stateMachine.ChangeState(EPlayerStates.IDLE);
        }



        public void StartGame()
        {
            ResetPlayer();
        }

        private void ResetPlayer()
        {
            HasFinishedLevel = false;
            HasDied = false;
            SetShapeImmediate(EShapeType.CUBE);
            _stateMachine.ChangeState(EPlayerStates.MOVE);
        }

        public void Tick()
        {
            if (HasFinishedLevel || HasDied) return;

            _playerInvisibleController.Update();
            _stateMachine.Update();
        }

        public void FixedTick()
        {
            if (HasFinishedLevel || HasDied) return;

            _stateMachine.FixedUpdate();
        }

        public void DrawGizmos()
        {
            _stateMachine.DrawGizmos();
        }


        public void HandleSwipe(ESwipeDirection swipeDirection)
        {
            if (!TryMapSwipeToShape(swipeDirection, out EShapeType nextShape))
            {
                return;
            }

            if (nextShape == CurrentShapeType) return;

            PlayFX(EParticleType.SHAPE_TRANSITION);

            EShapeType previousShape = CurrentShapeType;
            CurrentShapeType = nextShape;

            _view.PlayShapeTransition(previousShape, nextShape, swipeDirection);
            _config.SetCurrentShape(nextShape);

        }

        private void PlayFX(EParticleType type)
        {
            Vector3 offset = type == EParticleType.DEATH ? Vector3.down * 1.5f : Vector3.zero;
            _particleService.SpawnParticle(type, _view.transform.position , Quaternion.identity);
        }

        public void CheckCollision(BlockView block)
        {
            bool isValidCollision = (int)CurrentShapeType == (int)block.BlockType;

            if (!isValidCollision)
            {
                Die();
                return;
            }

            OnCrossedWall.Invoke(block);
            _playerInvisibleController.NextBlock();
        }



        private void Die()
        {

            PlayFX(EParticleType.DEATH);

            _stateMachine.ChangeState(EPlayerStates.IDLE);

            _view.HideHighlight();
            _view.HideAllShapes();
            HasDied = true;

            OnDied.Invoke();
        }

        public void ReachFinishLine()
        {
            HasFinishedLevel = true;
            _stateMachine.ChangeState(EPlayerStates.IDLE);
            PlayFX(EParticleType.CONFETTI);
            OnFinishedLevel.Invoke();
        }

        public void SetShapeImmediate(EShapeType shapeType)
        {
            CurrentShapeType = shapeType;
            _view.ShowShape(shapeType);
            _config.SetCurrentShape(shapeType);
        }

        public void HideHighlight()
        {
            _view.HideHighlight();
        }

        public void SetVelocity(Vector3 velocity)
        {
            _view.Rigidbody.linearVelocity = velocity;
        }
        public void SetPlayerPhysicsEnabled(bool enabled)
        { 
            _view.Rigidbody.isKinematic = !enabled;
        }

        public void ShowHighlight(Vector3 position, Color color, EShapeType shapeType)
        {
            _view.ShowHightLight(position, color, shapeType);
        }
        public void SetAllShapesRigidbodyKinematic(bool isKinematic)
        {
            _view.SetAllShapeRigidbodyKinematic(isKinematic);
        }

        public void DisableShapeColliders()
        {
            _view.SetShapeCollidersEnabled(false);
        }
        public void ClearShapeConstraints()
        {
            _view.ClearShapeContraints();
        }



        public void Cleanup()
        {
            _stateMachine.CleanUp();
            _view.HideHighlight();

            _playerInvisibleController.CleanUp();
        }

        private static bool TryMapSwipeToShape(ESwipeDirection swipeDirection, out EShapeType shapeType)
        {
            
            shapeType = swipeDirection switch
            {
                ESwipeDirection.UP => EShapeType.CYLINDER,
                ESwipeDirection.LEFT => EShapeType.SPHERE,
                ESwipeDirection.DOWN => EShapeType.CUBE,
                ESwipeDirection.RIGHT => EShapeType.TRIANGLE,
                _ => default
            };

            return swipeDirection != ESwipeDirection.NONE;
        }

        public float GetDistanceSq(Transform fromTransform)
        {
            return Vector3.SqrMagnitude(Transform.position - fromTransform.position);
        }

    }
}
