using UnityEngine;

namespace Scripts.Player
{
    public class MoveState : BaseState
    {

        public override void OnEnterState() { }
        public override void OnStateExit() 
        {

        }
        public override void Update() {
            PlayerView.transform.position += PlayerView.transform.forward * PlayerConfig.m_MoveSpeed * Time.fixedDeltaTime;
        }
        public override void FixedUpdate() 
        {
        }
        public override void OnDestroy() { }
    }
}
