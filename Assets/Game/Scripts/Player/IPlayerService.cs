using System;
using UnityEngine;

namespace Scripts.Player
{
    public interface IPlayerService
    {
        PlayerConfig PlayerConfig { get; }
        public event Action OnPlayerFinishedLevel;
        public event Action OnPlayerDied;
        public event Action<BlockView> OnPlayerCrossedWall;
        public event Action<bool> OnPlayerBeastModeActivated;
        void SpawnPlayer(Vector3 position);
        void InitializeCamera(Vector3 spawnPosition);

        void Reset();
        void CleanUp();

        public  abstract void StartGame();
        public abstract void InvokePlayerDeath();
        public abstract void CheckCollision(BlockView blockView);
        public float GetPlayerDistanceToFinish();
        public float GetTotalDistanceToFinish();
    }
}
