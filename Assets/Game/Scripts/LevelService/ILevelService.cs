using System;
using UnityEngine;

namespace Scripts.Level
{
    public interface ILevelService
    {
        public event Action OnLevelCreated;
        public event Action OnLevelCompleted;
        void CreateLevel(int levelNo);
        LevelConfig GetLevel(int levelNo);

        int GetNextLevel();

        void InvokeLevelCompleted();

        void Cleanup();
    }
}
