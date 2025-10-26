using System;
using UnityEngine;

namespace Scripts.GameService
{
    public interface IGameLoopService
    {
         event Action OnStart;
         event Action OnUpdateTick;
         event Action OnFixedUpdateTick;
    }
}
