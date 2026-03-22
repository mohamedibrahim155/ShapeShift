using System;
using UnityEngine;

namespace Scripts.Player
{
    public interface IPlayerService
    {
        public event Action OnPlayerFinishedLevel;
        public event Action OnPlayerDied;
        void SpawnPlayer(Vector3 position);
        void InitializeCamera(Vector3 spawnPosition);

        void Reset();
        void CleanUp();

        public  abstract void StartGame();
        public abstract void InvokePlayerDeath();

        public float GetPlayerProgressedDistance();
        public float GetTotalProgressedDistance();
    }
}
