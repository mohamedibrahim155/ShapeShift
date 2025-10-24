using UnityEngine;
using Zenject;

namespace Scripts.GameService
{
    public class GameService : IGameService
    {

        [Inject]
        private void Construct()
        {
            Debug.Log("GameService Constructed");
        }
       
        
        public void Initialize()
        {

        }
   
    }
}
