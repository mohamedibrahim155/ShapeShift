using UnityEngine;

namespace Scripts.Player
{
    public class FallState : BaseState
    {

        private float _timer;
        public override void OnEnterState() 
        {
            EnableKinematicPhysics(false);
        }
        public override void Update()
        {
            _timer += Time.deltaTime;

            if (_timer  >  PlayerConfig.m_FallTimer)
            {
                StateMachine.ChangeState(EPlayerStates.IDLE);
                return;
            }
        }

        public override void OnDestroy() { }

        private void EnableKinematicPhysics(bool value)
        {
            PlayerView.Rigidbody.isKinematic = value;
            foreach (var item in PlayerView.ShapeViews)
            {
                item.Value.Rigidbody.isKinematic = value;
            }
        }
    }
}
