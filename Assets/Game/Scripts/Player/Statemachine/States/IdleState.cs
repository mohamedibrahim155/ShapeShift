using Scripts.UI;
using UnityEngine;
using Zenject;

namespace Scripts.Player
{
    public class IdleState : BaseState
    {
        private float waitTimer;

        [Inject] private IUiService uiService;
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
           

        }
    }
}
