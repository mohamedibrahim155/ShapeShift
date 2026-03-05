using System;
using UnityEngine;

namespace Scripts.Level
{
    public interface ILevelService
    {
        public event Action OnLevelCreated;
        public event Action<int> OnLevelCompleted;
        void CreateLevel(int levelNo);
        LevelConfig GetLevel(int levelNo);

        void UpdateLevel(int levelNo);

        int GetWrappedLevelIndex(int levelNumber);
        int GetCurrentLevel();

        void SpawnLevel(int levelNumber);
        void InvokeLevelCompleted();

        void Cleanup();
    }
}
