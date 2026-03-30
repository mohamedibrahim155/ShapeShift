using UnityEngine;

namespace Scripts.GameplayStates
{
    public interface IGameplayState
    {
        void BeginState();
        void EndState();
    }
}
