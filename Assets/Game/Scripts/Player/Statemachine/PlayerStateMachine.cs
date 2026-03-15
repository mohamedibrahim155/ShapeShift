using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Scripts.Player
{

    public class PlayerStateMachine
    {
        public PlayerStateMachine(PlayerView PlayerView, PlayerConfig PlayerConfig)
        {
             this.PlayerView = PlayerView;
             this.PlayerConfig = PlayerConfig;
        }

        private Dictionary<EPlayerStates, BaseState> ListOfStates = new Dictionary<EPlayerStates, BaseState>();
        public EPlayerStates CurrentStateID { get; private set; }


        private PlayerView PlayerView;
        private PlayerConfig PlayerConfig;

        public void AddState(EPlayerStates stateID, BaseState state)
        {
            state.Setup(PlayerView, PlayerConfig, this);

            ListOfStates.Add(stateID, state);
        }
        public void ChangeState(EPlayerStates state)
        {
            if (CurrentStateID != EPlayerStates.NONE)
            {
                GetCurrentState().OnStateExit();
            }

            CurrentStateID = state;

            if (CurrentStateID != EPlayerStates.NONE)
            {
                GetCurrentState().OnEnterState();
            }
        }


        public void Update() 
        {
            GetCurrentState().Update();

        }
        public void FixedUpdate()
        {
            GetCurrentState().FixedUpdate();
        }

        public void DrawGizmos() 
        {

            GetCurrentState().DrawGizmos();
            
        }

        public void RemoveState(EPlayerStates stateID)
        {
            if (ListOfStates.ContainsKey(stateID))
            {
                ListOfStates[stateID].OnDestroy();
                ListOfStates.Remove(stateID);
            }
        }

        public void CleanUp()
        {
            foreach (var state in ListOfStates.Values)
            {
                state.OnDestroy();
            }
            ListOfStates.Clear();
        }

        public BaseState GetCurrentState() => ListOfStates[CurrentStateID];
    }
}
