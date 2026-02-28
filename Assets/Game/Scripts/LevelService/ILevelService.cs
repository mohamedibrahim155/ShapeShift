using System;
using UnityEngine;

namespace Scripts.Level
{
    public interface ILevelService
    {
        public event Action OnLevelCreated;
        void CreateLevel(int levelNo);
        LevelConfig GetLevel(int levelNo);


    }
}
