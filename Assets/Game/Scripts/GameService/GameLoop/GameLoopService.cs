using System;
using UnityEngine;
using Zenject;

namespace Scripts.GameService
{
    public class GameLoopService : IGameLoopService
    {
        public event Action OnStart = delegate { };
        public event Action OnUpdateTick= delegate{ };
        public event Action OnFixedUpdateTick = delegate { };
                                           
        [Inject]
        private void Construct()
        {
            new GameObject("GameLoop").AddComponent<GameLoopView>().Initalize(this);
        }

        public void Update()
        {
            OnUpdateTick?.Invoke();
        }

        public void FixedUpdate()
        {
            OnFixedUpdateTick?.Invoke();
        }
    }
}
