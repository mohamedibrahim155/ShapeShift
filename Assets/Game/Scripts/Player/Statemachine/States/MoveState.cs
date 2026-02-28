using UnityEngine;

namespace Scripts.Player
{
    public class MoveState : BaseState
    {

        public override void OnEnterState() 
        {
            ShowDebug = true;
        }
        public override void OnStateExit() 
        {
            PlayerView.Rigidbody.linearVelocity = Vector3.zero;
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
            PlayerView.Rigidbody.linearVelocity = PlayerView.transform.forward * PlayerConfig.m_MoveSpeed;
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

    }
}
