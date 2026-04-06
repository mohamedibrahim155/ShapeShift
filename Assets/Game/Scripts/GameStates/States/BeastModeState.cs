using Scripts.Player;
using System;
using UnityEngine;

namespace Scripts.GameplayStates
{
    public class BeastModeState : IGameplayState
    {
        private readonly IPlayerService _playerService;
        private readonly Action<bool> _onBeastModeActivated = delegate { };
        public BeastModeState(IPlayerService playerService, ref Action<bool> beastAction)
        { 
            _playerService = playerService;
            _onBeastModeActivated = beastAction;
        }

        bool m_isBeastActive = false;
        public void BeginState()
        {
            _onBeastModeActivated?.Invoke(true);
        }

        public void EndState()
        {
            _onBeastModeActivated?.Invoke(false);

        }

    }
}
