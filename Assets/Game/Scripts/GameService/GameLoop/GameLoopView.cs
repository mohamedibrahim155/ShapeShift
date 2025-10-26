
using UnityEngine;
using UnityEngine.LowLevel;
using Zenject;

namespace Scripts.GameService
{
    public class GameLoopView : MonoBehaviour
    {

        private GameLoopService _gameLoop;

        public void Initalize(GameLoopService gameLoop)
        {
            _gameLoop = gameLoop;
        }

        public void Start() => _gameLoop.Start();


        public void Update() => _gameLoop.Update();
     

        public void FixedUpdate() => _gameLoop.FixedUpdate();
       
    }

}