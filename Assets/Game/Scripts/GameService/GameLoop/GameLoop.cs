using System;
using UnityEngine;
using Zenject;

namespace Scripts.GameService
{
    public class GameLoop : IGameLoop
    {
        public event Action OnStart = delegate { };
        public event Action OnUpdate= delegate{ };
        public event Action OnFixedUpdate = delegate { };
                                           
        [Inject]
        private void Construct()
        {
            new GameObject("GameLoop").AddComponent<GameLoopView>().Initalize(this);
        }

        public void Start()
        {
            OnStart?.Invoke();
        }

        public void Update()
        {
            OnUpdate?.Invoke();
        }

        public void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
    }
}
