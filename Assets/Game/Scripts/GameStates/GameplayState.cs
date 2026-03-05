using UnityEngine;

namespace Scripts.GameplayStates
{
    public interface GameplayState
    {
        void BeginState();
        void EndState();
    }
}
