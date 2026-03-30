using UnityEngine;

namespace Scripts.GameplayStates
{
    public class PausedState : IGameplayState
    {
        //Pause the game by setting time scale to 0, and unpause by setting it back to 1.
        public void BeginState()
        {
            Time.timeScale = 0;
        }

        public void EndState()
        {
            Time.timeScale = 1;
        }
    }
}
