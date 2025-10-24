using System;
using UnityEngine;

namespace Scripts.GameService
{
    public interface IGameLoop
    {
         event Action OnStart;
         event Action OnUpdate;
         event Action OnFixedUpdate;
    }
}
