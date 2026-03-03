using UnityEngine;
using Zenject;

namespace Scripts.Player
{
    public class IdleState : BaseState
    {
        private float waitTimer;

        [Inject] private IPlayerService m_PlayerService;
        public override void OnEnterState() 
        {
            waitTimer = 0f;
        }
        public override void OnStateExit() { }
        public override void Update() 
        {
            HandleLevelFinished();
        }
        public override void FixedUpdate() { }
        public override void OnDestroy() { }

        private void HandleLevelFinished()
        {
            if (PlayerConfig.m_HasPlayerFinished)
            {
                if (waitTimer > PlayerConfig.m_FinishLineWaitTimer)
                {
                    m_PlayerService.Reset();
                    return;
                }

                waitTimer += Time.deltaTime;
            }

        }
    }
}
