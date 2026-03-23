using System;
using UnityEngine;

namespace Scripts.GameService
{
    public interface IGameLoopService
    {
        event Action OnUpdateTick;
        event Action OnFixedUpdateTick;
        event Action OnGizemosTick;
        event Action OnDestroyed;
    }
}
