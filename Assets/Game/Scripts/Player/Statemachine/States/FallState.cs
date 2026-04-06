using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Scripts.Player
{
    public class FallState : BaseState
    {

        private float _timer;

        private readonly ICameraService _cameraService;
        private readonly PlayerController _playerController;
        public FallState(ICameraService cameraService, PlayerController playerController)
        {
            _cameraService = cameraService;
            _playerController = playerController;
        }

        public override void OnEnterState() 
        {
            _timer = 0;

            //disables colliders to prevent further interactions with the player while falling and enables finish line camera for cinematic effect
            _playerController.SetPlayerPhysicsEnabled(true);
            _playerController.DisableShapeColliders();
            _playerController.SetAllShapesRigidbodyKinematic(false);
            _playerController.ClearShapeConstraints();
          

            // change camera to finish line camera for cinematic effect
            _cameraService.EnableCamera(ECameraType.FINISHLINE_CAMERA);
            _cameraService.SetCameraLookAt(null);
            _cameraService.SetCameraFollow(null);
        }
        public override void Update()
        {
            _timer += Time.deltaTime;

            if (_timer  >  PlayerConfig.m_FallTimer)
            {
                StateMachine.ChangeState(EPlayerStates.IDLE);
                return;
            }

            _playerController.SetVelocity(_playerController.Transform.forward * PlayerConfig.m_MoveSpeed);

        }
    }
}
