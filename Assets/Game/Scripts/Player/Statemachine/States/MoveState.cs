using UnityEngine;

namespace Scripts.Player
{
    public class MoveState : BaseState
    {
        private readonly PlayerInvisibleController m_PlayerInvisibleController;
        private readonly PlayerController m_PlayerController;

        private Vector3 playerForward;
        private float playerMoveSpeed;
        public MoveState(PlayerInvisibleController playerInvisibleController, PlayerController playerController)
        {
            m_PlayerInvisibleController = playerInvisibleController;
            m_PlayerController = playerController;
        }

        public override void OnEnterState() 
        {
            ShowDebug = true;

            playerForward = PlayerView.transform.forward;
            playerMoveSpeed = PlayerConfig.m_MoveSpeed;
        }



        public override void OnStateExit() 
        {
            m_PlayerController.SetVelocity(Vector3.zero);
            m_PlayerInvisibleController.Hide();
        }
        public override void Update() 
        {
        }
        public override void FixedUpdate() 
        {
            if (!IsGrounded())
            {
                StateMachine.ChangeState(EPlayerStates.FALL);
                return;
            }

            m_PlayerController.SetVelocity(playerForward * playerMoveSpeed);
        }
        public override void DrawGizmos() 
        {
                base.DrawGizmos();
                Gizmos.color = Color.red;
                Gizmos.DrawLine(m_PlayerController.Transform.position, m_PlayerController.Transform.position + Vector3.down * PlayerConfig.m_GroundCheckDistance);
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(m_PlayerController.Transform.position, Vector3.down, PlayerConfig.m_GroundCheckDistance, PlayerConfig.m_GroundLayer);
        }

        private void UpdatePlayerBasedOnPosition(Vector3 direction)
        {
            PlayerView.transform.position +=   direction   * Time.deltaTime;

        }

    }
}
