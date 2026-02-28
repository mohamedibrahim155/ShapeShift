using UnityEngine;

namespace Scripts.Player
{
    public class BaseState
    {
        public virtual void OnEnterState() { }
        public virtual void OnStateExit() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnDestroy() { }
        public virtual void DrawGizmos() { }



        public void Setup(PlayerView playerview, PlayerConfig config, PlayerStateMachine stateMachine)
        {
            PlayerView = playerview;
            PlayerConfig = config;
            StateMachine = stateMachine;
        }

        public void ChangeState(EPlayerStates newState) => StateMachine.ChangeState(newState);

        protected PlayerView PlayerView;
        protected PlayerConfig PlayerConfig;
        protected PlayerStateMachine StateMachine;

        public bool ShowDebug { get; set; } = false;

    }
}
