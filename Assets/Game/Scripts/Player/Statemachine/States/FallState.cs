using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Scripts.Player
{
    public class FallState : BaseState
    {

        private float _timer;

        private readonly ICameraService _cameraService;

        public FallState(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        public override void OnEnterState() 
        {
            _timer = 0;
            EnableKinematicPhysics(false);

            //disables colliders to prevent further interactions with the player while falling and enables finish line camera for cinematic effect
            PlayerView.DisbaleColliders();
            // change camera to finish line camera for cinematic effect
            _cameraService.EnableCamera(ECameraType.FINISHLINE_CAMERA);

            ResetCamera();
        }
        public override void Update()
        {
            _timer += Time.deltaTime;

            if (_timer  >  PlayerConfig.m_FallTimer)
            {
                StateMachine.ChangeState(EPlayerStates.IDLE);
                return;
            }

            PlayerView.Rigidbody.linearVelocity = PlayerView.transform.forward * PlayerConfig.m_MoveSpeed;

        }

        private void EnableKinematicPhysics(bool value)
        {
            PlayerView.Rigidbody.isKinematic = value;
            foreach (var item in PlayerView.GetShapes())
            {
                item.Value.Rigidbody.isKinematic = value;
                item.Value.Rigidbody.constraints = RigidbodyConstraints.None;
            }
        }

        private void ResetCamera()
        {
            _cameraService.SetCameraLookAt(null);
            _cameraService.SetCameraFollow(null);
        }
    }
}
