using Scripts.Level;
using Scripts.Player;
using System;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Scripts.Level
{

    public class FinishLine : MonoBehaviour
    {
        public Collider Collider;


       private PlayerConfig _config;
       private ILevelService _levelService;

        [Inject]
        public void Construct(ILevelService levelService, PlayerConfig config)
        {
            _levelService = levelService;
            _config = config;

            _levelService.InitializeFinishLine(this);
        }

        private void Reset()
        {
            Collider = GetComponentInChildren<Collider>();
        }

        private void OnTriggerExit(Collider other)
        {
            if ((_config.m_PlayerLayer & (1 << other.gameObject.layer)) != 0)
            {
                OnPlayerReachedFinishLine();
            }

        }

        private void OnPlayerReachedFinishLine()
        {
            _levelService.InvokeLevelCompleted();
        }
    }
}
