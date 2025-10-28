using System;
using UnityEngine;

namespace Scripts.Player
{
    public interface IPlayerService
    {
        void SpawnPlayer(Vector3 position);
        void RegisterBlockWall(BlockView wall);
    }
}
