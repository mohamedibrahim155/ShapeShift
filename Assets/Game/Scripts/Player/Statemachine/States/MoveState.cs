using UnityEngine;

namespace Scripts.Player
{
    public class MoveState : BaseState
    {

        private Vector3 playerForward;
        private float playerMoveSpeed;
        private readonly PlayerInvisibleController m_PlayerInvisibleController;
        private readonly IPlayerService m_PlayerService;
        public MoveState(PlayerInvisibleController playerInvisibleController, IPlayerService playerService)
        {
            m_PlayerInvisibleController = playerInvisibleController;
            this.m_PlayerService = playerService;
        }

        public override void OnEnterState() 
        {
            ShowDebug = true;

            playerForward = PlayerView.transform.forward;
            playerMoveSpeed = PlayerConfig.m_MoveSpeed;
            m_PlayerService.OnPlayerCrossedWall += OnPlayerCrossedWall;
        }

        private void OnPlayerCrossedWall(BlockView obj)
        {
            m_PlayerInvisibleController.NextBlock();
        }

        public override void OnStateExit() 
        {
            UpdatePlayerMovement(Vector3.zero);
            m_PlayerInvisibleController.Hide();
            m_PlayerService.OnPlayerCrossedWall -= OnPlayerCrossedWall;
        }
        public override void Update() 
        {
            m_PlayerInvisibleController.Update();
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
