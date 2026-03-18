using System;
using UnityEngine;

namespace Scripts.Level
{
    public interface ILevelService
    {
        public event Action OnLevelCreated;
        public event Action OnLevelCompleted;
        public event Action OnLevelFailed;

        public FinishLine FinishLineView { get; }
        void CreateLevel(int levelNo);
        LevelConfig GetLevel(int levelNo);

        void UpdateLevel(int levelNo);

        int GetWrappedLevelIndex(int levelNumber);
        int GetCurrentLevel();

        void SpawnLevel(int levelNumber);
        void SpawnNextLevel();
        void InvokeLevelCompleted();
        void InvokeLevelFailed();

        void InitializeFinishLine(FinishLine view);

        void Cleanup();
    }
}
