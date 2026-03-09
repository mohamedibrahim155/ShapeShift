using UnityEngine;

namespace Scripts.Player
{
    public class MoveState : BaseState
    {

        private Vector3 playerForward;
        private float playerMoveSpeed;
        public override void OnEnterState() 
        {
            ShowDebug = true;

            playerForward = PlayerView.transform.forward;
            playerMoveSpeed = PlayerConfig.m_MoveSpeed;
        }
        public override void OnStateExit() 
        {
            UpdatePlayerMovement(Vector3.zero);
        }
        public override void Update() {
           
        }
        public override void FixedUpdate() 
        {
            if (!IsGrounded())
            {
                StateMachine.ChangeState(EPlayerStates.FALL);
                return;
            }

            UpdatePlayerMovement(playerForward * playerMoveSpeed);
        }
        public override void DrawGizmos() 
        {
                base.DrawGizmos();
                Gizmos.color = Color.red;
                Gizmos.DrawLine(PlayerView.transform.position, PlayerView.transform.position + Vector3.down * PlayerConfig.m_GroundCheckDistance);
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(PlayerView.transform.position, Vector3.down, PlayerConfig.m_GroundCheckDistance, PlayerConfig.m_GroundLayer);
        }

        private void UpdatePlayerMovement(Vector3 velocity)
        {

            PlayerView.Rigidbody.linearVelocity = velocity;

        }

        private void UpdatePlayerBasedOnPosition(Vector3 direction)
        {
            PlayerView.transform.position +=   direction   * Time.deltaTime;

        }

    }
}
