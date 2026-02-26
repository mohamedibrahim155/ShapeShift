using UnityEngine;

namespace Scripts.Player
{
    public class MoveState : BaseState
    {

        public override void OnEnterState() { }
        public override void OnStateExit() 
        {
            PlayerView.Rigidbody.linearVelocity = Vector3.zero;
        }
        public override void Update() {
            PlayerView.Rigidbody.linearVelocity = PlayerView.transform.forward * PlayerConfig.m_MoveSpeed;
        }
        public override void FixedUpdate() 
        {
        }
        public override void OnDestroy() { }
    }
}
